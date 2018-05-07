using NetmqRouter.Models;

namespace NetmqRouter.Infrastructure
{
    internal interface IDataContractBuilder : IDataContractAccess
    {
        void RegisterRoute(Route route);

        void RegisterSubscriber(Subsriber subsriber);

        void RegisterSerializer<T>(ITypeSerializer<T> typeSerializer);
        void RegisterGeneralSerializer<T>(IGeneralSerializer<T> serializer);
    }
}