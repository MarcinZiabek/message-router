using System.Collections.Generic;
using NetmqRouter.Models;

namespace NetmqRouter.Infrastructure
{
    internal interface IDataContractAccess
    {
        IReadOnlyList<Route> Routes { get; }
        IReadOnlyList<Subsriber> Subscribers { get; }
        IReadOnlyList<Serializer> Serializers { get; }
    }
}