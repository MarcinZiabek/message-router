using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace NetmqRouter
{
    public class MessageRouter : IMessageRouter
    {
        private List<Route> _registeredRoutes  = new List<Route>();
        private Dictionary<string, List<Route>> _routing;

        public PublisherSocket PublisherSocket { get; private set; }
        public SubscriberSocket SubscriberSocket { get; private set; }

        public int NumberOfWorkes { get; private set; } = 4;

        private ConcurrentQueue<Message> _incomingMessages = new ConcurrentQueue<Message>();
        private ConcurrentQueue<Message> _outcommingMessages = new ConcurrentQueue<Message>();

        public MessageRouter()
        {

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
                if (!_outcommingMessages.TryDequeue(out var message))
                    await Task.Delay(TimeSpan.FromMilliseconds(1));

                PublisherSocket.SendMessage(message);
            }
        }

        private async void SubscriberWorker()
        {
            while (true)
            {
                if (!SubscriberSocket.TryReceiveMessage(out var message))
                    await Task.Delay(TimeSpan.FromMilliseconds(1));

                _incomingMessages.Enqueue(message);
            }
        }

        private async void MessageHandlerWorker()
        {
            while (true)
            {
                if(!_incomingMessages.TryDequeue(out var message))
                    await Task.Delay(TimeSpan.FromMilliseconds(1));


            }
        }

        public void StartRouting()
        {
            _routing = _registeredRoutes
                .GroupBy(x => x.IncomingRouteName)
                .ToDictionary(x => x.Key, x => x.ToList());

            Task.Run(() => PublisherWorker());
            Task.Run(() => SubscriberWorker());
        }
    }
}
