using System.Collections.Generic;
using MessageRouter.Models;

namespace MessageRouter.Infrastructure
{
    internal interface IDataContractAccess
    {
        IReadOnlyList<Route> Routes { get; }
        IReadOnlyList<Subscriber> Subscribers { get; }
        IReadOnlyList<Serializer> Serializers { get; }
    }
}