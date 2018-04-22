using System.Collections.Concurrent;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    internal class MessageSender : WorkerClassBase
    {
        private readonly IConnection _connection;
        private readonly ConcurrentQueue<Message> _messageQueue = new ConcurrentQueue<Message>();

        private readonly ITextSerializer _textSerializer;
        private readonly IObjectSerializer _objectSerializer;
        
        public MessageSender(IConnection connection, ITextSerializer textSerializer, IObjectSerializer objectSerializer)
        {
            _connection = connection;
            
            _textSerializer = textSerializer;
            _objectSerializer = objectSerializer;
        }

        public void SendMessage(Message message) => _messageQueue.Enqueue(message);
        
        protected override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var message))
                return false;

            byte[] dataBuffer = null;

            if (message.Payload == null)
                dataBuffer = null;
            
            else if (message.Payload is string text)
                dataBuffer = _textSerializer.Serialize(text);
            
            else if (message.Payload is byte[] data)
                dataBuffer = data;
            
            else if (message.Payload is object _object)
                dataBuffer = _objectSerializer.Serialize(_object);
            
            var serializedMessage = new SerializedMessage(message.RouteName, dataBuffer);
            _connection.SendMessage(serializedMessage);
            return true;
        }
    }
}