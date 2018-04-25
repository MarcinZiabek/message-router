using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NetmqRouter.Models;

[assembly: InternalsVisibleTo("NetmqRouter.Tests")]

namespace NetmqRouter.Infrastructure
{
    public interface IConnection
    {
        void Connect(IEnumerable<string> routeNames);
        void Disconnect();
        
        void SendMessage(SerializedMessage message);
        bool TryReceiveMessage(out SerializedMessage message);
    }
}