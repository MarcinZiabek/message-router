using System;

namespace NetmqRouter.Infrastructure
{
    internal interface IExceptionSource
    {
        event Action<Exception> OnException;
    }
}