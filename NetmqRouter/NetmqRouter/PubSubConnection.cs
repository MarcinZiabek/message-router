using NetMQ.Sockets;

namespace NetmqRouter
{
    public class PubSubConnection : IConnection
    {
        public PublisherSocket PublisherSocket { get; }
        public SubscriberSocket SubscriberSocket { get; }

        public PubSubConnection(PublisherSocket publisherSocket, SubscriberSocket subscriberSocket)
        {
            PublisherSocket = publisherSocket;
            SubscriberSocket = subscriberSocket;
        }

        public void SendMessage(Message message) => PublisherSocket.SendMessage(message);

        public bool TryReceiveMessage(out Message message) => SubscriberSocket.TryReceiveMessage(out message);

        public void Connect()
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