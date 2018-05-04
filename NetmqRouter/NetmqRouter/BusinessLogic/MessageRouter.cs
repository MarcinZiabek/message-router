using System;
using System.Linq;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetMQ.Sockets;

namespace NetmqRouter.BusinessLogic
{
    public class MessageRouter : IDisposable
    {
        internal readonly IDataContractBuilder DataContractBuilder = new DataContractBuilder();
        internal readonly IConnection Connection;
        internal readonly DataFlowManager DataFlowManager = new DataFlowManager();

        internal int NumberOfSerializationWorkes = 1;
        internal int NumberOfHandlingWorkes = 4;

        private MessageRouter(IConnection connection)
        {
            Connection = connection;
        }

        #region Managing

        public MessageRouter StartRouting()
        {
            var dataContract = new DataContractManager(DataContractBuilder);
            Connection.Connect(dataContract.GetIncomingRouteNames());

            DataFlowManager.CreateWorkers(Connection, dataContract);
            DataFlowManager.RegisterDataFlow();
            DataFlowManager.StartWorkers(NumberOfSerializationWorkes, NumberOfHandlingWorkes);

            return this;
        }

        public MessageRouter StopRouting()
        {
            DataFlowManager.StopWorkers();
            return this;
        }

        public void Dispose()
        {
            StopRouting();
            Disconnect();
        }

        public MessageRouter Disconnect()
        {
            Connection.Disconnect();
            return this;
        }

        #endregion

        #region Connection

        public static MessageRouter WithPubSubConnecton(PublisherSocket publisherSocket, SubscriberSocket subscriberSocket)
        {
            var connection = new PubSubConnection(publisherSocket, subscriberSocket);
            return new MessageRouter(connection);
        }

        public static MessageRouter WithPubSubConnecton(string publishAddress, string subscribeAddress)
        {
            var connection = new PubSubConnection(new PublisherSocket(publishAddress), new SubscriberSocket(subscribeAddress));
            return new MessageRouter(connection);
        }

        #endregion

        #region Serialization

        public MessageRouter RegisterTypeSerializerFor<T>(ITypeSerializer<T> typeSerializer)
        {
            DataContractBuilder.RegisterSerializer(typeSerializer);
            return this;
        }

        public MessageRouter RegisterGeneralSerializerFor<T>(IGeneralSerializer<T> serializer)
        {
            DataContractBuilder.RegisterGeneralSerializer(serializer);
            return this;
        }

        #endregion

        #region Routing

        public MessageRouter RegisterRoute(string routeName, Type dataType)
        {
            DataContractBuilder.RegisterRoute(new Route(routeName, dataType));
            return this;
        }

        #endregion

        #region Subscription

        public MessageRouter Subscribe<T>(T subscriber)
        {
            ClassAnalyzer
                .AnalyzeClass(subscriber)
                .ToList()
                .ForEach(DataContractBuilder.RegisterSubscriber);

            return this;
        }

        public MessageRouter RegisterSubscriber<T>(string routeName, Action<T> action)
        {
            var subscriber = Subsriber.Create(routeName, action);
            DataContractBuilder.RegisterSubscriber(subscriber);
            return this;
        }

        public MessageRouter RegisterSubscriber<T, TK>(string incomingRouteName, string outcomingRouteName, Func<T, TK> action)
        {
            var subscriber = Subsriber.Create(incomingRouteName, outcomingRouteName, action);
            DataContractBuilder.RegisterSubscriber(subscriber);
            return this;
        }

        #endregion

        #region Messaging

        internal void SendMessage(Message message) => DataFlowManager.SendMessage(message);

        public MessageRouter WithWorkerPool(int numberOfSerializationWorkes, int numberOfHandlingWorkes)
        {
            NumberOfSerializationWorkes = numberOfSerializationWorkes;
            NumberOfHandlingWorkes = numberOfHandlingWorkes;

            return this;
        }

        public void SendMessage(string routeName)
        {
            SendMessage(new Message(routeName, null));
        }

        public void SendMessage(string routeName, byte[] data)
        {
            SendMessage(new Message(routeName, data));
        }

        public void SendMessage(string routeName, string text)
        {
            SendMessage(new Message(routeName, text));
        }

        public void SendMessage(string routeName, object _object)
        {
            SendMessage(new Message(routeName, _object));
        }

        #endregion
    }
}
