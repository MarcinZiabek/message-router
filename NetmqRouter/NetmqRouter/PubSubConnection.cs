using System.Collections.Generic;
using System.Linq;
using NetmqRouter.Infrastructure;
using NetMQ.Sockets;

namespace NetmqRouter
{
    public class PubSubConnection : IConnection
    {
        PublisherSocket PublisherSocket { get; }
        SubscriberSocket SubscriberSocket { get; }

        public PubSubConnection(PublisherSocket publisherSocket, SubscriberSocket subscriberSocket)
        {
            PublisherSocket = publisherSocket;
            SubscriberSocket = subscriberSocket;
        }

        public void SendMessage(Message message) => PublisherSocket.SendMessage(message);

        public bool TryReceiveMessage(out Message message) => SubscriberSocket.TryReceiveMessage(out message);

        public void Connect(IEnumerable<string> routeNames)
        {
            routeNames
                .ToList()
                .ForEach(SubscriberSocket.Subscribe);
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