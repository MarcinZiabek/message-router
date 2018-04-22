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

        internal ITextSerializer _textSerializer = new BasicTextSerializer();
        internal IObjectSerializer _objectSerializer = new JsonObjectSerializer();

        private MessageSender _messageSender;
        private List<IWorkerTask> _workers = new List<IWorkerTask>();

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
            _messageSender.SendMessage(message);
        }

        public IMessageRouter StartRouting()
        {
            Connection.Connect(_registeredRoutes.Select(x => x.IncomingRouteName).Distinct());

            var typeContract = _registeredRoutes
                .ToDictionary(x => x.IncomingRouteName, x => x.Method.GetParameters()[0].ParameterType);
            
            var receiverWorker = new MessageReveiver(Connection, typeContract, _textSerializer, _objectSerializer);
            _messageSender = new MessageSender(Connection, _textSerializer, _objectSerializer);

            var handlerWorkers = Enumerable
                .Range(0, NumberOfWorkes)
                .Select(_ =>
                {
                    var handlerWorker = new MessageHandler(_registeredRoutes);
                    receiverWorker.OnNewMessage += handlerWorker.HandleMessage;
                    return handlerWorker;
                });
            
            _workers.AddRange(new IWorkerTask[] { receiverWorker, _messageSender });
            _workers.AddRange(handlerWorkers);

            _workers.ForEach(x => x.Start());
            return this;
        }

        public IMessageRouter StopRouting()
        {
            _workers.ForEach(x => x.Stop());
            return this;
        }

        public IMessageRouter Disconnect()
        {
            Connection.Disconnect();
            return this;
        }
    }
}
