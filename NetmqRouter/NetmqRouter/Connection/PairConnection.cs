using System.Collections.Generic;
using NetmqRouter.Helpers;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetMQ.Sockets;

namespace NetmqRouter.Connection
{
    public class PairConnection : IConnection
    {
        PairSocket Socket { get; }
        private readonly object _socketLock = new object();

        public PairConnection(PairSocket socket)
        {
            Socket = socket;
        }

        public void SendMessage(SerializedMessage message)
        {
            lock(_socketLock)
                Socket.SendMessage(message);
        }

        public bool TryReceiveMessage(out SerializedMessage message)
        {
            lock(_socketLock)
                return Socket.TryReceiveMessage(out message);
        }

        public void Connect(IEnumerable<string> routeNames)
        {

        }

        public void Disconnect()
        {
            Socket?.Close();
            Socket?.Dispose();
        }
    }
}