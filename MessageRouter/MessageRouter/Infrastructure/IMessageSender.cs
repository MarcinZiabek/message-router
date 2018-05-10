using MessageRouter.Models;

namespace MessageRouter.Infrastructure
{
    public interface IMessageSender
    {
        void SendMessage(Message message);
    }
}