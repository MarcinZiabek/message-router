using System;
using System.Collections.Generic;
using System.Linq;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.BusinessLogic
{
    internal class DataContract : IDataContract
    {
        internal List<Route> Routes = new List<Route>();
        internal List<RouteSubsriber> Subscribers { get; } = new List<RouteSubsriber>();
        internal Dictionary<Type, ISerializer> Serializers { get; } = new Dictionary<Type, ISerializer>();

        public void RegisterSerializer(Type targetType, ISerializer serializer)
        {
            if(Serializers.ContainsKey(targetType))
                throw new NetmqRouterException($"Serializer for type {targetType} is already registered!");
            
            Serializers.Add(targetType, serializer);
        }
        
        public void RegisterRoute(Route route)
        {
            if(!Serializers.ContainsKey(route.DataType))
                throw new NetmqRouterException($"Can not register route with type {route.DataType} because there is no reserialize for it.");
            
            if(Routes.Any(x => x.Name == route.Name))
                throw new NetmqRouterException($"Route with name {route.Name} is already registered.");
            
            Routes.Add(route);
        }

        public void RegisterSubscriber(RouteSubsriber routeSubsriber)
        {
            if(!Routes.Contains(routeSubsriber.Incoming))
                throw new NetmqRouterException($"Subscriber refers to not existing route (incoming) type and thereferore can not be registered.");
            
            if(routeSubsriber.Outcoming != null && !Routes.Contains(routeSubsriber.Outcoming))
                throw new NetmqRouterException($"Subscriber refers to not existing route type (outcoming) and thereferore can not be registered.");
            
            Subscribers.Add(routeSubsriber);
        }
        
        public IEnumerable<string> GetIncomingRouteNames()
        {
            return Subscribers
                .Select(x => x.Incoming.Name)
                .Distinct();
        }

        public IEnumerable<Message> CallRoute(Message message)
        {
            return Subscribers
                .Where(x => x.Incoming.Name == message.RouteName)
                .Select(x =>
                {
                    var response = x.Method(message.Payload);
                    return (x.Outcoming == null) ? null : new Message(x.Outcoming.Name, response);
                })
                .Where(x => x != null);
        }
        
        public SerializedMessage Serialize(Message message)
        {
            var targetType = Routes
                .First(x => x.Name == message.RouteName)
                .DataType;

            var data = Serializers[targetType].Serialize(message.Payload);
            return new SerializedMessage(message.RouteName, data);
        }
        
        public Message Deserialize(SerializedMessage message)
        {
            var targetType = Routes
                .First(x => x.Name == message.RouteName)
                .DataType;
            
            var payload = Serializers[targetType].Deserialize(message.Data, targetType);
            return new Message(message.RouteName, payload);
        }
    }
}