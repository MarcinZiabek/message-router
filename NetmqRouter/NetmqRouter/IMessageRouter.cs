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
        IMessageRouter StopRouting();
        IMessageRouter Disconnect();

        void SendMessage(string routeName);
        void SendMessage(string routeName, byte[] data);
        void SendMessage(string routeName, string text);
        void SendMessage(string routeName, object _object);
    }
}
