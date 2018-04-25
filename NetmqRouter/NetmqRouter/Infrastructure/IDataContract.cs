using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NetmqRouter.Models;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace NetmqRouter.Infrastructure
{
    internal interface IDataContract
    {
        void RegisterRoute(Route route);
        void RegisterSubscriber(RouteSubsriber routeSubsriber);
        void RegisterSerializer(Type targetType, ISerializer serializer);
        
        IEnumerable<string> GetIncomingRouteNames();
        bool IsIncomingRouteAllowed(string routeName);
        
        IEnumerable<Message> CallRoute(Message message);
        SerializedMessage Serialize(Message message);
        Message Deserialize(SerializedMessage message);
    }
}