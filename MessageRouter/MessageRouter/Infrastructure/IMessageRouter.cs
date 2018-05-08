using System;

namespace MessageRouter.Infrastructure
{
    public interface IMessageRouter : IMessageRouterManager, IMessageRouterConfiguration, IMessageSender, IExceptionSource, IDisposable
    {

    }
}