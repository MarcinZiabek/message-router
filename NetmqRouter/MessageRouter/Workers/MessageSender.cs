using System;
using System.Collections.Concurrent;
using NetmqRouter.Exceptions;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.Workers
{
    /// <summary>
    /// This worker is responsible for sending messages through the output socket.
    /// </summary>
    internal class MessageSender : WorkerClassBase
    {
        private readonly IConnection _connection;
        private readonly ConcurrentQueue<SerializedMessage> _messageQueue = new ConcurrentQueue<SerializedMessage>();

        public MessageSender(IConnection connection)
        {
            _connection = connection;
        }

        public void SendMessage(SerializedMessage message) => _messageQueue.Enqueue(message);

        internal override bool DoWork()
        {
            try
            {
                if (!_messageQueue.TryDequeue(out var message))
                    return false;

                _connection.SendMessage(message);
            }
            catch (Exception e)
            {
                throw new ConnectionException("Cannot send a message", e);
            }

            return true;
        }
    }
}