using System;
using System.Collections.Generic;
using System.Linq;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.BusinessLogic
{
    public class TypeComparer : IComparer<Type>
    {
        public int Compare(Type typeA, Type typeB)
        {
            if (typeA == typeB)
                return 0;

            return typeA.IsSubclassOf(typeB) ? 1 : -1;
        }
    }

    internal class DataContract : IDataContract
    {
        private readonly List<Route> _routes = new List<Route>();
        private readonly List<RouteSubsriber> _subscribers = new List<RouteSubsriber>();
        private readonly SortedList<Type, ISerializer> _serializers  = new SortedList<Type, ISerializer>(new TypeComparer());

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

        public IEnumerable<string> GetIncomingRouteNames()
        {
            return _subscribers
                .Select(x => x.Incoming.Name)
                .Distinct();
        }

        public IEnumerable<Message> CallRoute(Message message)
        {
            return _subscribers
                .Where(x => x.Incoming.Name == message.RouteName)
                .Select(x =>
                {
                    var response = x.Method(message.Payload);
                    return (x.Outcoming == null) ? null : new Message(x.Outcoming.Name, response);
                })
                .Where(x => x != null);
        }

        internal ISerializer FindSerializer(Type targetType)
        {
            return _serializers
                .FirstOrDefault(x => x.Key == targetType || targetType.IsSubclassOf(x.Key))
                .Value;
        }

        internal (ISerializer serializer, Type targetType) FindSerializer(string routeName)
        {
            var targetType = _routes
                .First(x => x.Name == routeName)
                .DataType;

            return (FindSerializer(targetType), targetType);
        }

        public SerializedMessage Serialize(Message message)
        {
            var (serializer, _) = FindSerializer(message.RouteName);
            var data = serializer.Serialize(message.Payload);
            return new SerializedMessage(message.RouteName, data);
        }

        public Message Deserialize(SerializedMessage message)
        {
            var (serializer, targetType) = FindSerializer(message.RouteName);
            var payload = serializer.Deserialize(message.Data, targetType);
            return new Message(message.RouteName, payload);
        }
    }
}