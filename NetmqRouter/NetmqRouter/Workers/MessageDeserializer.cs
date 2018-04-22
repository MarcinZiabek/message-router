using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        
        protected override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var serializedMessage))
                return false;

            var targetType = _dataContract
                .Routes
                .First(x => x.IncomingRouteName == serializedMessage.RouteName)
                .Method
                .GetParameters()
                ?[0]
                ?.ParameterType;
            
            object _object = null;
            
            if (targetType != null && serializedMessage.Data != null)
            {
                var serializer = _dataContract.Serialization[targetType];
                _object = serializer.Deserialize(serializedMessage.Data, targetType);
            }

            OnNewMessage?.Invoke(new Message(serializedMessage.RouteName, _object));
            return true;
        }
    }
}