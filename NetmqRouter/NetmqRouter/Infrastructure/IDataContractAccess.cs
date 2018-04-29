using System;
using System.Collections.Generic;
using NetmqRouter.Models;

namespace NetmqRouter.Infrastructure
{
    internal interface IDataContractAccess
    {
        IReadOnlyList<Route> Routes { get; }
        IReadOnlyList<RouteSubsriber> Subscribers { get; }
        IReadOnlyList<Serializer> Serializers { get; }
    }
}