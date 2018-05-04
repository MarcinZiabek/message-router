using NetmqRouter.BusinessLogic;

namespace NetmqRouter.Infrastructure
{
    public interface IMessageRouterManager
    {
        MessageRouter StartRouting();
        MessageRouter StopRouting();

        MessageRouter Disconnect();
    }
}