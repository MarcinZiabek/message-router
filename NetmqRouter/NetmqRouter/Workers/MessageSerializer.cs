using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    internal class MessageSerializer : WorkerClassBase
    {
        private readonly DataContract _dataContract;
        private readonly ConcurrentQueue<Message> _messageQueue = new ConcurrentQueue<Message>();

        public event Action<SerializedMessage> OnNewMessage;

        public MessageSerializer(DataContract dataContract)
        {
            _dataContract = dataContract;
        }
        
        public void SerializeMessage(Message message) => _messageQueue.Enqueue(message);
        
        internal override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var message))
                return false;
            
            byte[] dataBuffer = null;
            var targetType = message.Payload?.GetType();
            
            if (targetType != null && message.Payload != null)
            {
                var serializer = _dataContract.Serialization[targetType];
                dataBuffer = serializer.Serialize(message.Payload);
            }
            
            var serializedMessage = new SerializedMessage(message.RouteName, dataBuffer);
            OnNewMessage?.Invoke(serializedMessage);

            return true;
        }
    }
}