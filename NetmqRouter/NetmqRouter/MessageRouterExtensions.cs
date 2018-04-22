
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter
{
    public static class MessageRouterExtensions
    {
        public static void SendMessage(this IMessageRouter router, string routeName)
        {
            router.SendMessage(new Message(routeName, null));
        }

        public static void SendMessage(this IMessageRouter router, string routeName, byte[] data)
        {
            router.SendMessage(new Message(routeName, data));
        }

        public static void SendMessage(this IMessageRouter router, string routeName, string text)
        {
            router.SendMessage(new Message(routeName, text));
        }
        
        public static void SendMessage(this IMessageRouter router, string routeName, object _object)
        {
            router.SendMessage(new Message(routeName, _object));
        }
        
        public static void RegisterSerializerForType<T>(this MessageRouter router, ISerializer serializer)
        {
            router._dataSerializationContract.Add(typeof(T), serializer);
        }
    }
}