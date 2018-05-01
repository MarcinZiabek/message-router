using System;
using System.Collections.Concurrent;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    internal class MessageDeserializer : WorkerClassBase
    {
        private readonly IDataContractOperations _dataContractOperations;
        private readonly ConcurrentQueue<SerializedMessage> _messageQueue = new ConcurrentQueue<SerializedMessage>();
        
        public event Action<Message> OnNewMessage;

        public MessageDeserializer(IDataContractOperations dataContractOperations)
        {
            _dataContractOperations = dataContractOperations;
        }
        
        public void DeserializeMessage(SerializedMessage message) => _messageQueue.Enqueue(message);
        
        internal override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var serializedMessage))
                return false;

            var message = _dataContractOperations.Deserialize(serializedMessage);
            OnNewMessage?.Invoke(message);
            
            return true;
        }
    }
}