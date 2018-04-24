using System.Collections.Concurrent;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    internal class MessageSender : WorkerClassBase
    {
        private readonly IConnection _connection;
        private readonly ConcurrentQueue<SerializedMessage> _messageQueue = new ConcurrentQueue<SerializedMessage>();

        public MessageSender(IConnection connection)
        {
            _connection = connection;
        }

        public void SendMessage(SerializedMessage message) => _messageQueue.Enqueue(message);
        
        internal override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var message))
                return false;

            _connection.SendMessage(message);
            return true;
        }
    }
}