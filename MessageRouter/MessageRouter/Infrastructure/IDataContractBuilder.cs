using MessageRouter.Models;

namespace MessageRouter.Infrastructure
{
    internal interface IDataContractBuilder : IDataContractAccess
    {
        void RegisterRoute(Route route);

        void RegisterSubscriber(Subscriber subscriber);

        void RegisterSerializer<T>(ITypeSerializer<T> typeSerializer);
        void RegisterGeneralSerializer<T>(IGeneralSerializer<T> serializer);
    }
}