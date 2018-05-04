using System;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetmqRouter.Workers;

namespace NetmqRouter.BusinessLogic
{
    internal class DataFlowManager : IExceptionSource
    {
        private MessageReveiver _messageReveiver;
        private MessageDeserializer _messageDeserializer;
        private MessageHandler _messageHandler;
        private MessageSerializer _messageSerializer;
        private MessageSender _messageSender;

        public event Action<Exception> OnException;

        public void CreateWorkers(IConnection connection, IDataContractOperations dataContractOperations)
        {
            _messageReveiver = new MessageReveiver(connection);
            _messageDeserializer = new MessageDeserializer(dataContractOperations);
            _messageHandler = new MessageHandler(dataContractOperations);
            _messageSerializer = new MessageSerializer(dataContractOperations);
            _messageSender = new MessageSender(connection);
        }

        public void RegisterExceptionsHandler()
        {
            _messageReveiver.OnException += OnException;
            _messageDeserializer.OnException += OnException;
            _messageHandler.OnException += OnException;
            _messageSerializer.OnException += OnException;
            _messageSender.OnException += OnException;
        }

        public void RegisterDataFlow()
        {
            _messageReveiver.OnNewMessage += _messageDeserializer.DeserializeMessage;
            _messageDeserializer.OnNewMessage += _messageHandler.HandleMessage;
            _messageHandler.OnNewMessage += _messageSerializer.SerializeMessage;
            _messageSerializer.OnNewMessage += _messageSender.SendMessage;
        }

        public void StartWorkers(int numberOfSerializationWorkers, int numberOfHandlingWorkers)
        {
            _messageReveiver.Start();
            _messageDeserializer.Start(numberOfSerializationWorkers);
            _messageHandler.Start(numberOfHandlingWorkers);
            _messageSerializer.Start(numberOfSerializationWorkers);
            _messageSender.Start();
        }

        public void StopWorkers()
        {
            _messageReveiver.Stop();
            _messageDeserializer.Stop();
            _messageHandler.Stop();
            _messageSerializer.Stop();
            _messageSender.Stop();
        }

        internal void SendMessage(Message message)
        {
            _messageSerializer.SerializeMessage(message);
        }
    }
}