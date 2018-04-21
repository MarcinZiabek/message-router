using System;
using NetmqRouter.Infrastructure;

namespace NetmqRouter.Workers
{
    public class MessageReveiver : WorkerClassBase
    {
        private readonly IConnection _connection;
        
        internal event Action<Message> OnNewMessage;
        
        public MessageReveiver(IConnection connection)
        {
            _connection = connection;
        }
        
        protected override bool DoWork()
        {
            if (!_connection.TryReceiveMessage(out var message))
                return false;

            OnNewMessage?.Invoke(message);
            return true;
        }
    }
}