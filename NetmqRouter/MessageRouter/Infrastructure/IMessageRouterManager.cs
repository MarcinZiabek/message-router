using NetmqRouter.BusinessLogic;

namespace NetmqRouter.Infrastructure
{
    public interface IMessageRouterManager
    {
        IMessageRouter StartRouting();
        IMessageRouter StopRouting();

        IMessageRouter Disconnect();
    }
}