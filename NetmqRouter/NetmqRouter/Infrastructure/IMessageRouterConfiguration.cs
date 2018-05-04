using System;
using NetmqRouter.BusinessLogic;

namespace NetmqRouter.Infrastructure
{
    public interface IMessageRouterConfiguration
    {
        MessageRouter WithWorkerPool(int numberOfSerializationWorkes, int numberOfHandlingWorkes);

        MessageRouter RegisterRoute(string routeName, Type dataType);

        MessageRouter RegisterTypeSerializerFor<T>(ITypeSerializer<T> typeSerializer);
        MessageRouter RegisterGeneralSerializerFor<T>(IGeneralSerializer<T> serializer);

        MessageRouter RegisterSubscriber<T>(T subscriber);
        MessageRouter RegisterSubscriber(string routeName, Action action);
        MessageRouter RegisterSubscriber<T>(string routeName, Action<T> action);
        MessageRouter RegisterSubscriber<T>(string incomingRouteName, string outcomingRouteName, Func<T> action);
        MessageRouter RegisterSubscriber<T, TK>(string incomingRouteName, string outcomingRouteName, Func<T, TK> action);
    }
}