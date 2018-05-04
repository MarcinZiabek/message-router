using System;

namespace NetmqRouter.Exceptions
{
    public class SerializationException : NetmqRouterException
    {
        public SerializationException()
        {
        }

        public SerializationException(string message) : base(message)
        {
        }

        public SerializationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}