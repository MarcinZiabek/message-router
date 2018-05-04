namespace NetmqRouter.Infrastructure
{
    public interface IMessageSender
    {
        void SendMessage(string routeName);
        void SendMessage(string routeName, byte[] data);
        void SendMessage(string routeName, string text);
        void SendMessage(string routeName, object _object);
    }
}