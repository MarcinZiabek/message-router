using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    public class MessageHandler : WorkerClassBase
    {
        private readonly List<Route> _routing;
        private readonly ConcurrentQueue<Message> _messageQueue = new ConcurrentQueue<Message>();

        public MessageHandler(IEnumerable<Route> routing)
        {
            _routing = routing.ToList();
        }

        public void HandleMessage(Message message) => _messageQueue.Enqueue(message);

        protected override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var message))
                return false;

            _routing
                .Where(x => x.IncomingRouteName == message.RouteName)
                .ToList()
                .ForEach(x => x.Call(message.Payload));

            return true;
        }
    }
}