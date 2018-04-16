using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace NetmqRouter
{
    public class MessageRouter : IMessageRouter, IDisposable
    {
        private readonly List<Route> _registeredRoutes  = new List<Route>();
        private Dictionary<string, List<Route>> _routing;

        public PublisherSocket PublisherSocket { get; private set; }
        public SubscriberSocket SubscriberSocket { get; private set; }

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
            PublisherSocket = publisherSocket;
            SubscriberSocket = subscriberSocket;
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

                PublisherSocket.SendMessage(message);
            }
        }

        private async void SubscriberWorker()
        {
            while (true)
            {
                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (!SubscriberSocket.TryReceiveMessage(out var message))
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

        public void StopRouting()
        {
            _cancellationTokenSource.Cancel();

            try
            {
                Task.WaitAll(_runningTasks.ToArray());
            }
            catch
            {

            }
        }

        public void Disconnect()
        {
            PublisherSocket?.Close();
            PublisherSocket?.Dispose();

            SubscriberSocket?.Close();
            SubscriberSocket?.Dispose();
        }
    }
}
