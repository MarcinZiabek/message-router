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
        
        private readonly ITextSerializer _textSerializer;
        private readonly IObjectSerializer _objectSerializer;
        
        public MessageDeserializer(Dictionary<string, Type> typeContract, ITextSerializer textSerializer, IObjectSerializer objectSerializer)
        {
            _typeContract = typeContract;
            
            _textSerializer = textSerializer;
            _objectSerializer = objectSerializer;
        }
        
        public void DeserializeMessage(SerializedMessage message) => _messageQueue.Enqueue(message);
        
        protected override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var serializedMessage))
                return false;

            var targetType = _typeContract[serializedMessage.RouteName];
            object _object = null;
            
            if (targetType == null)
                _object = null;
            
            else if (targetType == typeof(string))
                _object = _textSerializer.Desialize(serializedMessage.Data);
            
            else if (targetType == typeof(byte[]))
                _object = serializedMessage.Data;
            
            else
                _object = _objectSerializer.Desialize(serializedMessage.Data, targetType);

            OnNewMessage?.Invoke(new Message(serializedMessage.RouteName, _object));
            return true;
        }
    }
}