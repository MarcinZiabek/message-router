using System;
using System.Collections.Concurrent;
using NetmqRouter.Exceptions;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    /// <summary>
    /// This worker is responsible for deserializing all messages.
    /// </summary>
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

            try
            {
                var message = _dataContractOperations.Deserialize(serializedMessage);
                OnNewMessage?.Invoke(message);
            }
            catch (Exception e)
            {
                throw new SerializationException("Cannot deserialize the emessage.", e);
            }

            return true;
        }
    }
}