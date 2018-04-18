using NetMQ.Sockets;

namespace NetmqRouter
{
    public interface IConnection
    {
        PublisherSocket PublisherSocket { get; }
        SubscriberSocket SubscriberSocket { get; }

        void Connect();
        void Disconnect();
        
        void SendMessage(Message message);
        bool TryReceiveMessage(out Message message);
    }
}