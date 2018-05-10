using System;
using MessageRouter.Models;

namespace MessageRouter.Infrastructure
{
    public interface IMessageRouterConfiguration
    {
        IMessageRouter WithWorkerPool(int numberOfSerializationWorkes, int numberOfHandlingWorkes);

        IMessageRouter RegisterRoute(string routeName, Type dataType);

        IMessageRouter RegisterTypeSerializer<T>(ITypeSerializer<T> typeSerializer);
        IMessageRouter RegisterGeneralSerializer<T>(IGeneralSerializer<T> serializer);

        IMessageRouter RegisterSubscriber(Subscriber subscriber);
    }
}