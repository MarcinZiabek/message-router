using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetmqRouter.Attributes;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetmqRouter.Workers;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace NetmqRouter
{
    public partial class MessageRouter : IMessageRouter, IDisposable
    {
        internal readonly DataContract _dataContract = new DataContract();

        private IConnection Connection { get; set; }
        public int NumberOfWorkes { get; private set; } = 4;

        private MessageReveiver _messageReveiver;
        private MessageDeserializer _messageDeserializer;
        private MessageHandler _messageHandler;
        private MessageSerializer _messageSerializer;
        private MessageSender _messageSender;

        public void Dispose()
        {
            StopRouting();
            Disconnect();
        }

        private MessageRouter(IConnection connection)
        {
            Connection = connection;
        }

        public IMessageRouter WithWorkerPool(int numberOfWorkers)
        {
            NumberOfWorkes = numberOfWorkers;
            return this;
        }

        public IMessageRouter Subscribe<T>(T subscriber)
        {
            ClassAnalyzer
                .AnalyzeClass(subscriber)
                .ToList()
                .ForEach(_dataContract.RegisterSubscriber);

            return this;
        }
        
        public static IMessageRouter WithPubSubConnecton(PublisherSocket publisherSocket, SubscriberSocket subscriberSocket)
        {
            var connection = new PubSubConnection(publisherSocket, subscriberSocket);
            return new MessageRouter(connection);
        }
        
        public static IMessageRouter WithPubSubConnecton(string publishAddress, string subscribeAddress)
        {
            var connection = new PubSubConnection(new PublisherSocket(publishAddress), new SubscriberSocket(subscribeAddress));
            return new MessageRouter(connection);
        }

        internal void SendMessage(Message message)
        {
            _messageSerializer.SerializeMessage(message);
        }

        public IMessageRouter StartRouting()
        {
            var routeNames = _dataContract
                .Subscribers
                .SelectMany(x => new[] { x.Incoming.Name, x.Outcoming.Name })
                .Distinct();
                
            Connection.Connect(routeNames);

            _messageReveiver = new MessageReveiver(Connection);
            _messageDeserializer = new MessageDeserializer(_dataContract);
            _messageHandler = new MessageHandler(_dataContract);
            _messageSerializer = new MessageSerializer(_dataContract);
            _messageSender = new MessageSender(Connection);
            
            _messageReveiver.OnNewMessage += _messageDeserializer.DeserializeMessage;
            _messageDeserializer.OnNewMessage += _messageHandler.HandleMessage;
            _messageSerializer.OnNewMessage += _messageSender.SendMessage;
            
            _messageReveiver.Start();
            _messageDeserializer.Start();
            _messageHandler.Start(NumberOfWorkes);
            _messageSerializer.Start();
            _messageSender.Start();
            
            return this;
        }

        public IMessageRouter StopRouting()
        {
            _messageReveiver.Stop();
            _messageDeserializer.Stop();
            _messageHandler.Stop();
            _messageSerializer.Stop();
            _messageSender.Stop();
            
            return this;
        }

        public IMessageRouter Disconnect()
        {
            Connection.Disconnect();
            return this;
        }
    }
}
