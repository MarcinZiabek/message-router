using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace NetmqRouter
{
    public class MessageRouter : IMessageRouter, IDisposable
    {
        private readonly List<Route> _registeredRoutes  = new List<Route>();
        private Dictionary<string, List<Route>> _routing;

        private IConnection Connection { get; set; }

        public int NumberOfWorkes { get; private set; } = 4;

        private readonly ConcurrentQueue<Message> _incomingMessages = new ConcurrentQueue<Message>();
        private readonly ConcurrentQueue<Message> _outcommingMessages = new ConcurrentQueue<Message>();

        private List<Task> _runningTasks = new List<Task>();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancellationToken;

        public void Dispose()
        {
            StopRouting();
            Disconnect();
        }

        public MessageRouter(PublisherSocket publisherSocket, SubscriberSocket subscriberSocket)
        {
            Connection = new PubSubConnection(publisherSocket, subscriberSocket);
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

        private void SubscribeRoutingOnSocket()
        {
            _registeredRoutes
                .ForEach(x => Connection.SubscriberSocket.Subscribe(x.IncomingRouteName));
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
            var data = Encoding.ASCII.GetBytes(text);
            SendMessage(new Message(routeName, RouteDataType.Text, data));
        }

        public void SendMessage(string routeName, object _object)
        {
            var json = JsonConvert.SerializeObject(_object);
            var data = Encoding.ASCII.GetBytes(json);
            SendMessage(new Message(routeName, RouteDataType.Object, data));
        }

        private void SendMessage(Message message)
        {
            _outcommingMessages.Enqueue(message);
        }

        private async void PublisherWorker()
        {
            while (true)
            {
                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (!_outcommingMessages.TryDequeue(out var message))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
                    continue;
                }

                Connection.SendMessage(message);
            }
        }

        private async void SubscriberWorker()
        {
            while (true)
            {
                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (!Connection.TryReceiveMessage(out var message))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
                    continue;
                }
                    
                _incomingMessages.Enqueue(message);
            }
        }

        private async void MessageHandlerWorker()
        {
            while (true)
            {
                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (!_incomingMessages.TryDequeue(out var message))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
                    continue;
                }
                    
                if(!_routing.ContainsKey(message.RouteName))
                    continue;

                _routing
                    [message.RouteName]
                    .Where(x => x.IncomingDataType == message.DataType)
                    .ToList()
                    .ForEach(x => x.Call(message.Buffer));
            }
        }

        public IMessageRouter StartRouting()
        {
            _routing = _registeredRoutes
                .GroupBy(x => x.IncomingRouteName)
                .ToDictionary(x => x.Key, x => x.ToList());

            SubscribeRoutingOnSocket();

            _cancellationToken = _cancellationTokenSource.Token;

            RunTask(PublisherWorker);
            RunTask(SubscriberWorker);

            for (int i = 0; i < NumberOfWorkes; i++)
                RunTask(MessageHandlerWorker);

            return this;
        }

        private void RunTask(Action action)
        {
            var task = Task.Run(action, _cancellationToken);
            _runningTasks.Add(task);
        }

        public IMessageRouter StopRouting()
        {
            _cancellationTokenSource.Cancel();

            try
            {
                Task.WaitAll(_runningTasks.ToArray());
            }
            catch
            {

            }

            return this;
        }

        public IMessageRouter Disconnect()
        {
            Connection.Disconnect();
            return this;
        }
    }
}
