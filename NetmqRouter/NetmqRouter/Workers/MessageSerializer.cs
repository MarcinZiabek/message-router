using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    internal class MessageSerializer : WorkerClassBase
    {
        private readonly IDataContractOperations _dataContractOperations;
        private readonly ConcurrentQueue<Message> _messageQueue = new ConcurrentQueue<Message>();

        public event Action<SerializedMessage> OnNewMessage;

        public MessageSerializer(IDataContractOperations dataContractOperations)
        {
            _dataContractOperations = dataContractOperations;
        }
        
        public void SerializeMessage(Message message) => _messageQueue.Enqueue(message);
        
        internal override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var message))
                return false;
            
            var serializedMessage = _dataContractOperations.Serialize(message);
            OnNewMessage?.Invoke(serializedMessage);

            return true;
        }
    }
}