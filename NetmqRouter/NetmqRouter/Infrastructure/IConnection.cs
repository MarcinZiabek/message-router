using System.Collections.Generic;
using NetmqRouter.Models;

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