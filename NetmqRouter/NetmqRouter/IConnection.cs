using System.Collections.Generic;
using NetMQ;
using NetMQ.Sockets;

namespace NetmqRouter
{
    public interface IConnection
    {
        void Connect(IEnumerable<string> routeNames);
        void Disconnect();
        
        void SendMessage(Message message);
        bool TryReceiveMessage(out Message message);
    }
}