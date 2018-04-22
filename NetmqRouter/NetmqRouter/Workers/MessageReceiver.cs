using System;
using System.Collections.Generic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    internal class MessageReveiver : WorkerClassBase
    {
        private readonly IConnection _connection;
        
        internal event Action<SerializedMessage> OnNewMessage;
        
        public MessageReveiver(IConnection connection)
        {
            _connection = connection;
        }
        
        protected override bool DoWork()
        {
            if (!_connection.TryReceiveMessage(out var serializedMessage))
                return false;

            OnNewMessage?.Invoke(serializedMessage);
            return true;
        }
    }
}