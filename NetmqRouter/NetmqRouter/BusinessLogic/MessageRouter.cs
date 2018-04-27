using System;
using System.Linq;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetMQ.Sockets;

namespace NetmqRouter.BusinessLogic
{
    public partial class MessageRouter : IDisposable
    {
        internal readonly IDataContract _dataContract = new DataContract();
        internal readonly IConnection _connection;
        internal readonly DataFlowManager _dataFlowManager = new DataFlowManager();
        
        internal int _numberOfSerializationWorkes = 1;
        internal int _numberOfHandlingWorkes = 4;
        
        public void Dispose()
        {
            StopRouting();
            Disconnect();
        }

        private MessageRouter(IConnection connection)
        {
            _connection = connection;
        }

        public MessageRouter Subscribe<T>(T subscriber)
        {
            ClassAnalyzer
                .AnalyzeClass(subscriber)
                .ToList()
                .ForEach(_dataContract.RegisterSubscriber);

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

        internal void SendMessage(Message message) => _dataFlowManager.SendMessage(message);

        public MessageRouter StartRouting()
        {
            _connection.Connect(_dataContract.GetIncomingRouteNames());

            _dataFlowManager.CreateWorkers(_connection, _dataContract);
            _dataFlowManager.RegisterDataFlow();
            _dataFlowManager.StartWorkers(_numberOfSerializationWorkes, _numberOfHandlingWorkes);
            
            return this;
        }

        public MessageRouter StopRouting()
        {
            _dataFlowManager.StopWorkers();
            return this;
        }

        public MessageRouter Disconnect()
        {
            _connection.Disconnect();
            return this;
        }
    }
}
