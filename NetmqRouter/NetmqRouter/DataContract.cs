using System;
using System.Collections.Generic;
using NetmqRouter.Infrastructure;

namespace NetmqRouter
{
    internal class DataContract
    {
        public List<RouteSubsriber> Routes { get; }  = new List<RouteSubsriber>();
        public Dictionary<Type, ISerializer> Serialization { get; } = new Dictionary<Type, ISerializer>();

        public void RegisterRoute(RouteSubsriber routeSubsriber) => Routes.Add(routeSubsriber);

        public void RegisterSerializer(Type targetType, ISerializer serializer)
        {
            Serialization.Add(targetType, serializer);
        }
    }
}