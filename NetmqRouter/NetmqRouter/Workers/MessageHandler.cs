using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
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

            var re = _dataContractOperations
                .CallRoute(message)
                .ToList();


            re.ForEach(x => OnNewMessage?.Invoke(x));

            return true;
        }
    }
}