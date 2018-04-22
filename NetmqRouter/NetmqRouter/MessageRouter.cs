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
        private readonly List<Route> _registeredRoutes  = new List<Route>();

        private IConnection Connection { get; set; }
        public int NumberOfWorkes { get; private set; } = 4;

        internal readonly Dictionary<Type, ISerializer> _dataSerializationContract = new Dictionary<Type, ISerializer>();
        
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
            var routes = ClassAnalyzer.AnalyzeClass(subscriber);
            _registeredRoutes.AddRange(routes);

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

        public void SendMessage(Message message)
        {
            _messageSerializer.SerializeMessage(message);
        }

        public IMessageRouter StartRouting()
        {
            Connection.Connect(_registeredRoutes.Select(x => x.IncomingRouteName).Distinct());

            var typeContract = _registeredRoutes
                .ToDictionary(x => x.IncomingRouteName, x => x.Method.GetParameters()[0].ParameterType);
            
            _messageReveiver = new MessageReveiver(Connection);
            _messageDeserializer = new MessageDeserializer(typeContract, _dataSerializationContract);
            _messageHandler = new MessageHandler(_registeredRoutes);
            _messageSerializer = new MessageSerializer(_dataSerializationContract);
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
