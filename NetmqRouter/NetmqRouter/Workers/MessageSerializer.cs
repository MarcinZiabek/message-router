using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    public class MessageSerializer : WorkerClassBase
    {
        private readonly ConcurrentQueue<Message> _messageQueue = new ConcurrentQueue<Message>();
        private readonly Dictionary<Type, ISerializer> _dataSerializationContract;
        
        public event Action<SerializedMessage> OnNewMessage;

        public MessageSerializer(Dictionary<Type, ISerializer> dataSerializationContract)
        {
            _dataSerializationContract = dataSerializationContract;
        }
        
        public void SerializeMessage(Message message) => _messageQueue.Enqueue(message);
        
        protected override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var message))
                return false;
            
            byte[] dataBuffer = null;
            var targetType = message.Payload?.GetType();
            
            if (targetType != null && message.Payload != null)
            {
                var serializer = _dataSerializationContract[targetType];
                dataBuffer = serializer.Serialize(message.Payload);
            }
            
            var serializedMessage = new SerializedMessage(message.RouteName, dataBuffer);
            OnNewMessage?.Invoke(serializedMessage);

            return true;
        }
    }
}