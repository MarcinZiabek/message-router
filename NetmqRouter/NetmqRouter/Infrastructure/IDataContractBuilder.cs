using System;
using System.Collections.Generic;
using NetmqRouter.Models;

namespace NetmqRouter.Infrastructure
{
    internal interface IDataContractBuilder : IDataContractAccess
    {
        void RegisterRoute(Route route);

        void RegisterSubscriber(Subsriber subsriber);

        void RegisterSerializer<T>(ISerializer<T> serializer);
        void RegisterGeneralSerializer<T>(IGeneralSerializer<T> serializer);
    }
}