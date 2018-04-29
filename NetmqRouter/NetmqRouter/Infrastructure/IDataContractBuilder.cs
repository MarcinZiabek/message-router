using System;
using System.Collections.Generic;
using NetmqRouter.Models;

namespace NetmqRouter.Infrastructure
{
    internal interface IDataContractBuilder : IDataContractAccess
    {
        void RegisterRoute(Route route);
        void RegisterSubscriber(RouteSubsriber routeSubsriber);
        void RegisterSerializer<T>(ISerializer<T> serializer);
        void RegisterGeneralSerializer(Type targetType, IGeneralSerializer serializer);
    }
}