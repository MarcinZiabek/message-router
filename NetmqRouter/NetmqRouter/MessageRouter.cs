using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetmqRouter.Attributes;
using NetmqRouter.Infrastructure;
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

        private ITextSerializer _textSerializer = new BasicTextSerializer();
        private IObjectSerializer _objectSerializer = new JsonObjectSerializer();

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

        public void SendMessage(string routeName)
        {
            SendMessage(new Message(routeName, RouteDataType.Void, null));
        }

        public void SendMessage(string routeName, byte[] data)
        {
            SendMessage(new Message(routeName, RouteDataType.RawData, null));
        }

        public void SendMessage(string routeName, string text)
        {
            SendMessage(new Message(routeName, RouteDataType.Text, _textSerializer.Serialize(text)));
        }

        public void SendMessage(string routeName, object _object)
        {
            SendMessage(new Message(routeName, RouteDataType.Object, _objectSerializer.Serialize(_object)));
        }

        private void SendMessage(Message message)
        {
            _messageSender.SendMessage(message);
        }

        public IMessageRouter StartRouting()
        {
            Connection.Connect(_registeredRoutes.Select(x => x.IncomingRouteName).Distinct());

            var receiverWorker = new MessageReveiver(Connection);
            _messageSender = new MessageSender(Connection);

            var handlerWorkers = Enumerable
                .Range(0, NumberOfWorkes)
                .Select(_ =>
                {
                    var handlerWorker = new MessageHandler(_registeredRoutes, _textSerializer, _objectSerializer);
                    receiverWorker.OnNewMessage += handlerWorker.HandleMessage;
                    return handlerWorker;
                });
            
            _workers.AddRange(new IWorkerTask[] { receiverWorker, _messageSender });
            _workers.AddRange(handlerWorkers);

            _workers
                .ForEach(x => x.Start());
            
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
