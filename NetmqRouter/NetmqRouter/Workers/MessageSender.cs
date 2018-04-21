using System.Collections.Concurrent;
using NetmqRouter.Infrastructure;

namespace NetmqRouter.Workers
{
    internal class MessageSender : WorkerClassBase
    {
        private readonly IConnection _connection;
        private readonly ConcurrentQueue<Message> _messageQueue = new ConcurrentQueue<Message>();

        public MessageSender(IConnection connection)
        {
            _connection = connection;
        }

        public void SendMessage(Message message) => _messageQueue.Enqueue(message);
        
        protected override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var message))
                return false;

            _connection.SendMessage(message);
            return true;
        }
    }
}