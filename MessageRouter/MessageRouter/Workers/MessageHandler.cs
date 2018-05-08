using System;
using System.Collections.Concurrent;
using System.Linq;
using MessageRouter.Exceptions;
using MessageRouter.Infrastructure;
using MessageRouter.Models;

namespace MessageRouter.Workers
{
    /// <summary>
    /// This worker is responsible for handling all messages.
    /// Steps: send a deserialzied message to all route subscribers, gather responses and send them via an event.
    /// </summary>
    internal class MessageHandler : WorkerClassBase
    {
        private readonly IDataContractOperations _dataContractOperations;
        private readonly ConcurrentQueue<Message> _messageQueue = new ConcurrentQueue<Message>();

        public event Action<Message> OnNewMessage;

        public MessageHandler(IDataContractOperations dataContractOperations)
        {
            _dataContractOperations = dataContractOperations;
        }

        public void HandleMessage(Message message) => _messageQueue.Enqueue(message);

        internal override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var message))
                return false;

            try
            {
                _dataContractOperations
                    .CallRoute(message)
                    .ToList()
                    .ForEach(x => OnNewMessage?.Invoke(x));
            }
            catch (Exception e)
            {
                throw new SubscriberException("The subscriber code throws an exception.", e);
            }

            return true;
        }
    }
}