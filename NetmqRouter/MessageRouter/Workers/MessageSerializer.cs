using System;
using System.Collections.Concurrent;
using NetmqRouter.Exceptions;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    /// <summary>
    /// This worker is responsible for serializing all messages.
    /// </summary>
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

            try
            {
                var serializedMessage = _dataContractOperations.Serialize(message);
                OnNewMessage?.Invoke(serializedMessage);
            }
            catch (Exception e)
            {
                throw new SerializationException("Cannot serialize the message", e);
            }

            return true;
        }
    }
}