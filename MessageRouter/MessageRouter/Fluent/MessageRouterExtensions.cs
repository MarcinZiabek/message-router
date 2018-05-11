using System;
using System.Linq;
using MessageRouter.BusinessLogic;
using MessageRouter.Infrastructure;
using MessageRouter.Models;

namespace MessageRouter.Fluent
{
    public static class MessageRouterExtensions
    {
        #region Subscription

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

        public static IMessageRouter RegisterSubscriber(this IMessageRouter router, string incomingRouteName, string outcomingRouteName, Action action)
        {
            var subscriber = Subscriber.Create(incomingRouteName, outcomingRouteName, action);
            router.RegisterSubscriber(subscriber);
            return router;
        }

        public static IMessageRouter RegisterSubscriber<T>(this IMessageRouter router, string incomingRouteName, string outcomingRouteName, Action<T> action)
        {
            var subscriber = Subscriber.Create(incomingRouteName, outcomingRouteName, action);
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

        #endregion

        #region Subscription - Fluent

        public static SubscribeFluent Subscribe(this IMessageRouter router, string incomingRouteName)
        {
            return new SubscribeFluent(router, incomingRouteName);
        }

        #endregion

        #region Sending Messages

        public static void SendEvent(this IMessageSender sender, string routeName)
        {
            sender.SendMessage(new Message(routeName, null));
        }

        public static void SendMessage(this IMessageSender sender, string routeName, byte[] data)
        {
            sender.SendMessage(new Message(routeName, data));
        }

        public static void SendMessage(this IMessageSender sender, string routeName, string text)
        {
            sender.SendMessage(new Message(routeName, text));
        }

        public static void SendMessage(this IMessageSender sender, string routeName, object _object)
        {
            sender.SendMessage(new Message(routeName, _object));
        }
        
        #endregion
        
        #region Sending Messages - Fluent

        public static SendMessageFluent SendEvent(this IMessageSender sender)
        {
            return new SendMessageFluent(sender, null);
        }
        
        public static SendMessageFluent SendMessage(this IMessageSender sender, byte[] data)
        {
            return new SendMessageFluent(sender, data);
        }
        
        public static SendMessageFluent SendMessage(this IMessageSender sender, string text)
        {
            return new SendMessageFluent(sender, text);
        }
        
        public static SendMessageFluent SendMessage(this IMessageSender sender, object _object)
        {
            return new SendMessageFluent(sender, _object);
        }
        
        #endregion
    }
}