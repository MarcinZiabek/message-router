using NetmqRouter.Infrastructure;
using NetMQ.Sockets;

namespace NetmqRouter
{
    public partial class MessageRouter
    {
        public static IMessageRouter WithPubSubConnecton(PublisherSocket publisherSocket, SubscriberSocket subscriberSocket)
        {
            var connection = new PubSubConnection(publisherSocket, subscriberSocket);
            return new MessageRouter(connection);
        }
        
        public static IMessageRouter WithPubSubConnecton(string publishAddress, string subscribeAddress)
        {
            var connection = new PubSubConnection(new PublisherSocket(publishAddress), new SubscriberSocket(subscribeAddress));
            return new MessageRouter(connection);
        }
    }
}