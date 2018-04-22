using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using NetmqRouter.Infrastructure;

namespace NetmqRouter
{
    internal class DataContract
    {
        public List<Route> Routes { get; }  = new List<Route>();
        public Dictionary<Type, ISerializer> Serialization { get; } = new Dictionary<Type, ISerializer>();

        public void RegisterRoute(Route route) => Routes.Add(route);

        public void RegisterSerializer(Type targetType, ISerializer serializer)
        {
            Serialization.Add(targetType, serializer);
        }
    }
}