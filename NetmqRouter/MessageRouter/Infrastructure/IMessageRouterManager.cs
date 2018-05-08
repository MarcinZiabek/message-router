namespace MessageRouter.Infrastructure
{
    public interface IMessageRouterManager
    {
        IMessageRouter StartRouting();
        IMessageRouter StopRouting();

        IMessageRouter Disconnect();
    }
}