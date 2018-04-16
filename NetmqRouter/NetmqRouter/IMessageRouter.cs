using System;
using System.Collections.Generic;
using System.Text;

namespace NetmqRouter
{
    public interface IMessageRouter : IDisposable
    {
        IMessageRouter WithWorkerPool(int numberOfWorkers);
        IMessageRouter Subscribe<T>(T subscriber);

        IMessageRouter StartRouting();
    }
}
