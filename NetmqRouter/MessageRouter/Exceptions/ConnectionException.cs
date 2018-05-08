using System;

namespace MessageRouter.Exceptions
{
    public class ConnectionException : NetmqRouterException
    {
        public ConnectionException()
        {
        }

        public ConnectionException(string message) : base(message)
        {
        }

        public ConnectionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}