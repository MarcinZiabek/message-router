
using System;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter
{
    public static class MessageRouterExtensions
    {
        public static MessageRouter WithWorkerPool(this MessageRouter router, int numberOfSerializationWorkes, int numberOfHandlingWorkes)
        {
            router.NumberOfSerializationWorkes = numberOfSerializationWorkes;
            router.NumberOfHandlingWorkes = numberOfHandlingWorkes;

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

        public static MessageRouter RegisterRoute(this MessageRouter router, string routeName, Type dataType)
        {
            router.DataContractBuilder.RegisterRoute(new Route(routeName, dataType));
            return router;
        }

        public static MessageRouter RegisterTypeSerializerFor<T>(this MessageRouter router, ITypeSerializer<T> typeSerializer)
        {
            router.DataContractBuilder.RegisterSerializer(typeSerializer);
            return router;
        }

        public static MessageRouter RegisterGeneralSerializerFor<T>(this MessageRouter router, IGeneralSerializer<T> serializer)
        {
            router.DataContractBuilder.RegisterGeneralSerializer(serializer);
            return router;
        }

        public static MessageRouter RegisterSubscriber<T>(this MessageRouter router, string routeName, Action<T> action)
        {
            var subscriber = Subsriber.Create(routeName, action);
            router.DataContractBuilder.RegisterSubscriber(subscriber);
            return router;
        }

        public static MessageRouter RegisterSubscriber<T, TK>(this MessageRouter router, string incomingRouteName, string outcomingRouteName, Func<T, TK> action)
        {
            var subscriber = Subsriber.Create(incomingRouteName, outcomingRouteName, action);
            router.DataContractBuilder.RegisterSubscriber(subscriber);
            return router;
        }
    }
}