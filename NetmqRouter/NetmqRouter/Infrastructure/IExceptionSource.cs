using System;

namespace NetmqRouter.Infrastructure
{
    public interface IExceptionSource
    {
        event Action<Exception> OnException;
    }
}