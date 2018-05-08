using System.Collections.Generic;
using MessageRouter.Models;

namespace MessageRouter.Infrastructure
{
    /// <summary>
    /// This interface should be used for implementing custom communication layer.
    /// </summary>
    public interface IConnection
    {
        void Connect(IEnumerable<string> routeNames);
        void Disconnect();

        void SendMessage(SerializedMessage message);
        bool TryReceiveMessage(out SerializedMessage message);
    }
}