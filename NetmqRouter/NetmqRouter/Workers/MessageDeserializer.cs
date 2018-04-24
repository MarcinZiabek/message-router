using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    internal class MessageDeserializer : WorkerClassBase
    {
        private readonly DataContract _dataContract;
        private readonly ConcurrentQueue<SerializedMessage> _messageQueue = new ConcurrentQueue<SerializedMessage>();
        
        public event Action<Message> OnNewMessage;

        public MessageDeserializer(DataContract dataContract)
        {
            _dataContract = dataContract;
        }
        
        public void DeserializeMessage(SerializedMessage message) => _messageQueue.Enqueue(message);
        
        internal override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var serializedMessage))
                return false;

            var message = _dataContract.Deserialize(serializedMessage);
            OnNewMessage?.Invoke(message);
            
            return true;
        }
    }
}