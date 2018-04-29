using System;
using System.Collections.Generic;
using NetmqRouter.Models;

namespace NetmqRouter.Infrastructure
{
    internal interface IDataContractBuilder : IDataContractAccess
    {
        void RegisterRoute(Route route);
        void RegisterSubscriber(RouteSubsriber routeSubsriber);
        void RegisterSerializer(Type targetType, ISerializer serializer);
    }
}