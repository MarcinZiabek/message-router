using System;

namespace NetmqRouter.Exceptions
{
    public class SubscriberException : NetmqRouterException
    {
        public SubscriberException()
        {
        }

        public SubscriberException(string message) : base(message)
        {
        }

        public SubscriberException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}