using System;
using System.Collections.Generic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    public class MessageReveiver : WorkerClassBase
    {
        private readonly IConnection _connection;
        private readonly Dictionary<string, Type> _typeContract;
        
        private readonly ITextSerializer _textSerializer;
        private readonly IObjectSerializer _objectSerializer;
        
        internal event Action<Message> OnNewMessage;
        
        public MessageReveiver(IConnection connection, Dictionary<string, Type> typeContract, ITextSerializer textSerializer, IObjectSerializer objectSerializer)
        {
            _connection = connection;

            _typeContract = typeContract;
            
            _textSerializer = textSerializer;
            _objectSerializer = objectSerializer;
        }
        
        protected override bool DoWork()
        {
            if (!_connection.TryReceiveMessage(out var serializedMessage))
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