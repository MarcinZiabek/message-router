namespace NetmqRouter.Models
{
    internal struct Message
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
