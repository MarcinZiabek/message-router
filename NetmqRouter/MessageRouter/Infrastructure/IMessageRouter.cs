using System;

namespace NetmqRouter.Infrastructure
{
    public interface IMessageRouter : IMessageRouterManager, IMessageRouterConfiguration, IMessageSender, IExceptionSource, IDisposable
    {

    }
}