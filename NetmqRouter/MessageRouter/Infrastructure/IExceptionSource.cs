using System;

namespace MessageRouter.Infrastructure
{
    public interface IExceptionSource
    {
        event Action<Exception> OnException;
    }
}