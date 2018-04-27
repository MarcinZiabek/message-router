using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NetmqRouter.Tests")]

namespace NetmqRouter.Models
{
    internal class Message
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
