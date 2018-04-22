namespace NetmqRouter.Models
{
    public struct Message
    {
        public readonly string RouteName;
        public readonly object Payload;

        public Message(string routeName, object payload)
        {
            RouteName = routeName;
            Payload = payload;
        }
    }
}
