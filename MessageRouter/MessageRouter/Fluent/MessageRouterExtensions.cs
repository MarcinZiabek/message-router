using System;
using System.Linq;
using MessageRouter.BusinessLogic;
using MessageRouter.Infrastructure;
using MessageRouter.Models;

namespace MessageRouter.Fluent
{
    public static class MessageRouterExtensions
    {
        public static IMessageRouter RegisterSubscriber<T>(this IMessageRouter router, T subscriber)
        {
            ClassAnalyzer
                .AnalyzeClass(subscriber)
                .ToList()
                .ForEach(x => router.RegisterSubscriber(x));

            return router;
        }

        public static IMessageRouter RegisterSubscriber(this IMessageRouter router, string routeName, Action action)
        {
            var subscriber = Subscriber.Create(routeName, action);
            router.RegisterSubscriber(subscriber);
            return router;
        }

        public static IMessageRouter RegisterSubscriber<T>(this IMessageRouter router, string routeName, Action<T> action)
        {
            var subscriber = Subscriber.Create(routeName, action);
            router.RegisterSubscriber(subscriber);
            return router;
        }

        public static IMessageRouter RegisterSubscriber<T>(this IMessageRouter router, string incomingRouteName, string outcomingRouteName, Func<T> action)
        {
            var subscriber = Subscriber.Create(incomingRouteName, outcomingRouteName, action);
            router.RegisterSubscriber(subscriber);
            return router;
        }

        public static IMessageRouter RegisterSubscriber<T, TK>(this IMessageRouter router, string incomingRouteName, string outcomingRouteName, Func<T, TK> action)
        {
            var subscriber = Subscriber.Create(incomingRouteName, outcomingRouteName, action);
            router.RegisterSubscriber(subscriber);
            return router;
        }
        
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
    }
}