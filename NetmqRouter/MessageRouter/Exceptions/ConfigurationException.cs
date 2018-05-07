using System;

namespace NetmqRouter.Exceptions
{
    public class ConfigurationException : NetmqRouterException
    {
        public ConfigurationException()
        {
        }

        public ConfigurationException(string message) : base(message)
        {
        }

        public ConfigurationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}