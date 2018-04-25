
using System;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter
{
    public static class MessageRouterExtensions
    {
        public static MessageRouter WithWorkerPool(this MessageRouter router, int numberOfSerializationWorkes, int numberOfHandlingWorkes)
        {
            router._numberOfSerializationWorkes = numberOfSerializationWorkes;
            router._numberOfHandlingWorkes = numberOfHandlingWorkes;
            
            return router;
        }
        
        public static void SendMessage(this MessageRouter router, string routeName)
        {
            router.SendMessage(new Message(routeName, null));
        }

        public static void SendMessage(this MessageRouter router, string routeName, byte[] data)
        {
            router.SendMessage(new Message(routeName, data));
        }

        public static void SendMessage(this MessageRouter router, string routeName, string text)
        {
            router.SendMessage(new Message(routeName, text));
        }
        
        public static void SendMessage(this MessageRouter router, string routeName, object _object)
        {
            router.SendMessage(new Message(routeName, _object));
        }

        public static void RegisterRouter(this MessageRouter router, string routeName, Type dataType)
        {
            router._dataContract.RegisterRoute(new Route(routeName, dataType));
        }
        
        public static void RegisterSerializerForType<T>(this MessageRouter router, ISerializer serializer)
        {
            router._dataContract.RegisterSerializer(typeof(T), serializer);
        }
    }
}