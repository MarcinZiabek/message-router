using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    public class MessageDeserializer : WorkerClassBase
    {
        private readonly ConcurrentQueue<SerializedMessage> _messageQueue = new ConcurrentQueue<SerializedMessage>();
        public event Action<Message> OnNewMessage;
        
        private readonly Dictionary<string, Type> _typeContract;
        private readonly Dictionary<Type, ISerializer> _dataSerializationContract;

        public MessageDeserializer(Dictionary<string, Type> typeContract, Dictionary<Type, ISerializer> dataSerializationContract)
        {
            _typeContract = typeContract;
            _dataSerializationContract = dataSerializationContract;
        }
        
        public void DeserializeMessage(SerializedMessage message) => _messageQueue.Enqueue(message);
        
        protected override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var serializedMessage))
                return false;

            var targetType = _typeContract[serializedMessage.RouteName];
            object _object = null;
            
            if (targetType != null && serializedMessage.Data != null)
            {
                var serializer = _dataSerializationContract[targetType];
                _object = serializer.Deserialize(serializedMessage.Data, targetType);
            }

            OnNewMessage?.Invoke(new Message(serializedMessage.RouteName, _object));
            return true;
        }
    }
}