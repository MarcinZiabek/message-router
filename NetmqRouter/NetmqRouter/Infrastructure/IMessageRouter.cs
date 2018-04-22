using System;
using NetmqRouter.Models;

namespace NetmqRouter.Infrastructure
{
    public interface IMessageRouter : IDisposable
    {
        IMessageRouter WithWorkerPool(int numberOfWorkers);
        IMessageRouter Subscribe<T>(T subscriber);

        IMessageRouter StartRouting();
        IMessageRouter StopRouting();
        IMessageRouter Disconnect();
    }
}
