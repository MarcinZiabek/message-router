using System;
using System.Collections.Generic;
using System.Text;

namespace NetmqRouter
{
    public interface IMessageRouter
    {
        IMessageRouter WithWorkerPool(int numberOfWorkers);
        IMessageRouter Subscribe<T>(T subscriber);

        void StartRouting();
    }
}
