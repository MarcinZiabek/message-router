using System;
using System.Collections.Generic;
using System.Linq;
using NetmqRouter.Helpers;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.BusinessLogic
{
    internal class DataContractBuilder : IDataContractBuilder, IDataContractAccess
    {
        private readonly List<Route> _routes = new List<Route>();
        private readonly List<RouteSubsriber> _subscribers = new List<RouteSubsriber>();
        private readonly Dictionary<Type, ISerializer> _serializers  = new Dictionary<Type, ISerializer>();

        public IReadOnlyList<Route> Routes => _routes;
        public IReadOnlyList<RouteSubsriber> Subscribers => _subscribers;
        public IReadOnlyDictionary<Type, ISerializer> Serializers => _serializers;

        public void RegisterSerializer(Type targetType, ISerializer serializer)
        {
            if(_serializers.ContainsKey(targetType))
                throw new NetmqRouterException($"Serializer for type {targetType} is already registered!");

            _serializers.Add(targetType, serializer);
        }

        public void RegisterRoute(Route route)
        {
            if(!_serializers.ContainsKey(route.DataType))
                throw new NetmqRouterException($"Can not register route with type {route.DataType} because there is no serializer for it.");

            if(_routes.Any(x => x.Name == route.Name))
                throw new NetmqRouterException($"Route with name {route.Name} is already registered.");

            _routes.Add(route);
        }

        public void RegisterSubscriber(RouteSubsriber routeSubsriber)
        {
            if(!_routes.Contains(routeSubsriber.Incoming))
                throw new NetmqRouterException($"Subscriber refers to not existing route (incoming) type and thereferore can not be registered.");

            if(routeSubsriber.Outcoming != null && !_routes.Contains(routeSubsriber.Outcoming))
                throw new NetmqRouterException($"Subscriber refers to not existing route type (outcoming) and thereferore can not be registered.");

            _subscribers.Add(routeSubsriber);
        }
    }
}