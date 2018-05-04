using System.Collections.Generic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetMQ.Sockets;

namespace NetmqRouter.Connection
{
    public class PushPullConnection : IConnection
    {
        PushSocket PublisherSocket { get; }
        PullSocket SubscriberSocket { get; }

        public PushPullConnection(PushSocket publisherSocket, PullSocket subscriberSocket)
        {
            PublisherSocket = publisherSocket;
            SubscriberSocket = subscriberSocket;
        }

        public void SendMessage(SerializedMessage message) => PublisherSocket.SendMessage(message);

        public bool TryReceiveMessage(out SerializedMessage message) => SubscriberSocket.TryReceiveMessage(out message);

        public void Connect(IEnumerable<string> routeNames)
        {

        }

        public void Disconnect()
        {
            PublisherSocket?.Close();
            PublisherSocket?.Dispose();

            SubscriberSocket?.Close();
            SubscriberSocket?.Dispose();
        }
    }
}