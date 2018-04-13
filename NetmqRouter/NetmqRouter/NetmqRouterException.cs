using System;
using System.Collections.Generic;
using System.Text;

namespace NetmqRouter
{
    public class NetmqRouterException : Exception
    {
        public NetmqRouterException()
        {
        }

        public NetmqRouterException(string message) : base(message)
        {
        }

        public NetmqRouterException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
