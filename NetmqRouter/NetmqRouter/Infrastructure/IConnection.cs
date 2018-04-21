using System.Collections.Generic;

namespace NetmqRouter.Infrastructure
{
    public interface IConnection
    {
        void Connect(IEnumerable<string> routeNames);
        void Disconnect();
        
        void SendMessage(Message message);
        bool TryReceiveMessage(out Message message);
    }
}