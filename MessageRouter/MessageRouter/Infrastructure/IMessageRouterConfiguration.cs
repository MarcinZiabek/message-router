using System;

namespace MessageRouter.Infrastructure
{
    public interface IMessageRouterConfiguration
    {
        IMessageRouter WithWorkerPool(int numberOfSerializationWorkes, int numberOfHandlingWorkes);

        IMessageRouter RegisterRoute(string routeName, Type dataType);

        IMessageRouter RegisterTypeSerializer<T>(ITypeSerializer<T> typeSerializer);
        IMessageRouter RegisterGeneralSerializer<T>(IGeneralSerializer<T> serializer);

        IMessageRouter RegisterSubscriber<T>(T subscriber);
        IMessageRouter RegisterSubscriber(string routeName, Action action);
        IMessageRouter RegisterSubscriber<T>(string routeName, Action<T> action);
        IMessageRouter RegisterSubscriber<T>(string incomingRouteName, string outcomingRouteName, Func<T> action);
        IMessageRouter RegisterSubscriber<T, TK>(string incomingRouteName, string outcomingRouteName, Func<T, TK> action);
    }
}