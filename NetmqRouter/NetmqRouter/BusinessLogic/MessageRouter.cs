using System;
using System.Linq;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetMQ.Sockets;

namespace NetmqRouter.BusinessLogic
{
    public partial class MessageRouter : IDisposable
    {
        internal readonly IDataContractBuilder DataContractBuilder = new DataContractBuilder();
        internal readonly IConnection Connection;
        internal readonly DataFlowManager DataFlowManager = new DataFlowManager();

        internal int NumberOfSerializationWorkes = 1;
        internal int NumberOfHandlingWorkes = 4;

        public void Dispose()
        {
            StopRouting();
            Disconnect();
        }

        private MessageRouter(IConnection connection)
        {
            Connection = connection;
        }

        public MessageRouter Subscribe<T>(T subscriber)
        {
            ClassAnalyzer
                .AnalyzeClass(subscriber)
                .ToList()
                .ForEach(DataContractBuilder.RegisterSubscriber);

            return this;
        }

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

        internal void SendMessage(Message message) => DataFlowManager.SendMessage(message);

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

        public MessageRouter Disconnect()
        {
            Connection.Disconnect();
            return this;
        }
    }
}
