using System;

namespace NetmqRouter.Infrastructure
{
    public interface IMessageRouter : IDisposable
    {
        IMessageRouter Subscribe<T>(T subscriber);

        IMessageRouter StartRouting();
        IMessageRouter StopRouting();
        IMessageRouter Disconnect();
    }
}
