using MessageRouter.Infrastructure;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ
{
    public class NetmqMessageRouter
    {
        public static IMessageRouter WithPubSubConnecton(PublisherSocket publisherSocket, SubscriberSocket subscriberSocket)
        {
            var connection = new PubSubConnection(publisherSocket, subscriberSocket);
            return new MessageRouter.BusinessLogic.MessageRouter(connection);
        }

        public static IMessageRouter WithPubSubConnecton(string publishAddress, string subscribeAddress)
        {
            return WithPubSubConnecton(new PublisherSocket(publishAddress), new SubscriberSocket(subscribeAddress));
        }

        public static IMessageRouter WithPushPullConnection(PushSocket pushSocket, PullSocket pullSocket)
        {
            var connection = new PushPullConnection(pushSocket, pullSocket);
            return new MessageRouter.BusinessLogic.MessageRouter(connection);
        }

        public static IMessageRouter WithPushPullConnection(string pushAddress, string pullAddress)
        {
            return WithPushPullConnection(new PushSocket(pushAddress), new PullSocket(pullAddress));
        }

        public static IMessageRouter WithPairConnection(PairSocket socket)
        {
            var connection = new PairConnection(socket);
            return new MessageRouter.BusinessLogic.MessageRouter(connection);
        }

        public static IMessageRouter WithPairConnection(string address)
        {
            return WithPairConnection(new PairSocket(address));
        }
    }
}