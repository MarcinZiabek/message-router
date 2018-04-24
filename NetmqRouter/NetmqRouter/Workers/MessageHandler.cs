using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    internal class MessageHandler : WorkerClassBase
    {
        private readonly DataContract _dataContract;
        private readonly ConcurrentQueue<Message> _messageQueue = new ConcurrentQueue<Message>();

        public MessageHandler(DataContract dataContract)
        {
            _dataContract = dataContract;
        }

        public void HandleMessage(Message message) => _messageQueue.Enqueue(message);

        internal override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var message))
                return false;

            _dataContract
                .Routes
                .Where(x => x.IncomingRouteName == message.RouteName)
                .ToList()
                .ForEach(x => x.Call(message.Payload));

            return true;
        }
    }
}