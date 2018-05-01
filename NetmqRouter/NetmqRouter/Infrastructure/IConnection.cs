using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NetmqRouter.Models;

[assembly: InternalsVisibleTo("NetmqRouter.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace NetmqRouter.Infrastructure
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