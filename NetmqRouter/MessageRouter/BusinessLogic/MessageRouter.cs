using System;
using System.Linq;
using NetmqRouter.Connection;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetMQ.Sockets;

namespace NetmqRouter.BusinessLogic
{
    public class MessageRouter : IMessageRouter
    {
        private readonly IDataContractBuilder _dataContractBuilder = new DataContractBuilder();
        private readonly IConnection _connection;
        private readonly DataFlowManager _dataFlowManager = new DataFlowManager();

        private int _numberOfSerializationWorkes = 1;
        private int _numberOfHandlingWorkes = 4;

        public event Action<Exception> OnException;

        public MessageRouter(IConnection connection)
        {
            _connection = connection;
            _dataFlowManager.OnException += OnException;
        }

        #region Managing

        public IMessageRouter StartRouting()
        {
            var dataContract = new DataContractManager(_dataContractBuilder);
            _connection.Connect(dataContract.GetIncomingRouteNames());

            _dataFlowManager.CreateWorkers(_connection, dataContract);
            _dataFlowManager.RegisterExceptionsHandler();
            _dataFlowManager.RegisterDataFlow();
            _dataFlowManager.StartWorkers(_numberOfSerializationWorkes, _numberOfHandlingWorkes);

            return this;
        }

        public IMessageRouter StopRouting()
        {
            _dataFlowManager.StopWorkers();
            return this;
        }

        public void Dispose()
        {
            StopRouting();
            Disconnect();
        }

        public IMessageRouter Disconnect()
        {
            _connection.Disconnect();
            return this;
        }

        public IMessageRouter WithWorkerPool(int numberOfSerializationWorkes, int numberOfHandlingWorkes)
        {
            _numberOfSerializationWorkes = numberOfSerializationWorkes;
            _numberOfHandlingWorkes = numberOfHandlingWorkes;

            return this;
        }

        #endregion

        #region Connection

        public static IMessageRouter WithPubSubConnecton(PublisherSocket publisherSocket, SubscriberSocket subscriberSocket)
        {
            var connection = new PubSubConnection(publisherSocket, subscriberSocket);
            return new MessageRouter(connection);
        }

        public static IMessageRouter WithPubSubConnecton(string publishAddress, string subscribeAddress)
        {
            return WithPubSubConnecton(new PublisherSocket(publishAddress), new SubscriberSocket(subscribeAddress));
        }

        public static IMessageRouter WithPushPullConnection(PushSocket pushSocket, PullSocket pullSocket)
        {
            var connection = new PushPullConnection(pushSocket, pullSocket);
            return new MessageRouter(connection);
        }

        public static IMessageRouter WithPushPullConnection(string pushAddress, string pullAddress)
        {
            return WithPushPullConnection(new PushSocket(pushAddress), new PullSocket(pullAddress));
        }

        public static IMessageRouter WithPairConnection(PairSocket socket)
        {
            var connection = new PairConnection(socket);
            return new MessageRouter(connection);
        }

        public static IMessageRouter WithPairConnection(string address)
        {
            return WithPairConnection(new PairSocket(address));
        }

        #endregion

        #region Serialization

        public IMessageRouter RegisterTypeSerializer<T>(ITypeSerializer<T> typeSerializer)
        {
            _dataContractBuilder.RegisterSerializer(typeSerializer);
            return this;
        }

        public IMessageRouter RegisterGeneralSerializer<T>(IGeneralSerializer<T> serializer)
        {
            _dataContractBuilder.RegisterGeneralSerializer(serializer);
            return this;
        }

        #endregion

        #region Routing

        public IMessageRouter RegisterRoute(string routeName, Type dataType)
        {
            _dataContractBuilder.RegisterRoute(new Route(routeName, dataType));
            return this;
        }

        #endregion

        #region Subscription

        public IMessageRouter RegisterSubscriber<T>(T subscriber)
        {
            ClassAnalyzer
                .AnalyzeClass(subscriber)
                .ToList()
                .ForEach(_dataContractBuilder.RegisterSubscriber);

            return this;
        }

        public IMessageRouter RegisterSubscriber(string routeName, Action action)
        {
            var subscriber = Subsriber.Create(routeName, action);
            _dataContractBuilder.RegisterSubscriber(subscriber);
            return this;
        }

        public IMessageRouter RegisterSubscriber<T>(string routeName, Action<T> action)
        {
            var subscriber = Subsriber.Create(routeName, action);
            _dataContractBuilder.RegisterSubscriber(subscriber);
            return this;
        }

        public IMessageRouter RegisterSubscriber<T>(string incomingRouteName, string outcomingRouteName, Func<T> action)
        {
            var subscriber = Subsriber.Create(incomingRouteName, outcomingRouteName, action);
            _dataContractBuilder.RegisterSubscriber(subscriber);
            return this;
        }

        public IMessageRouter RegisterSubscriber<T, TK>(string incomingRouteName, string outcomingRouteName, Func<T, TK> action)
        {
            var subscriber = Subsriber.Create(incomingRouteName, outcomingRouteName, action);
            _dataContractBuilder.RegisterSubscriber(subscriber);
            return this;
        }

        #endregion

        #region Messaging

        internal void SendMessage(Message message) => _dataFlowManager.SendMessage(message);

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
