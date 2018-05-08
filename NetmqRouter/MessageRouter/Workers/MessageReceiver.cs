using System;
using MessageRouter.Exceptions;
using MessageRouter.Infrastructure;
using MessageRouter.Models;

namespace MessageRouter.Workers
{
    /// <summary>
    /// This worker is responsible for receiving messages from the input socket.
    /// </summary>
    internal class MessageReveiver : WorkerClassBase
    {
        private readonly IConnection _connection;

        internal event Action<SerializedMessage> OnNewMessage;

        public MessageReveiver(IConnection connection)
        {
            _connection = connection;
        }

        internal override bool DoWork()
        {
            try
            {
                if (!_connection.TryReceiveMessage(out var serializedMessage))
                    return false;

                OnNewMessage?.Invoke(serializedMessage);
            }
            catch (Exception e)
            {
                throw new ConnectionException("Cannot receive a message", e);
            }

            return true;
        }
    }
}