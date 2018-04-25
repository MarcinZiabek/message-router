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
        internal Dictionary<Type, ISerializer> Serialization { get; } = new Dictionary<Type, ISerializer>();

        public void RegisterRoute(Route route) => Routes.Add(route);
        
        public void RegisterSubscriber(RouteSubsriber routeSubsriber) => Subscribers.Add(routeSubsriber);

        public void RegisterSerializer(Type targetType, ISerializer serializer)
        {
            Serialization.Add(targetType, serializer);
        }

        public IEnumerable<string> GetIncomingRouteNames()
        {
            return Subscribers
                .Select(x => x.Incoming.Name)
                .Distinct();
        }
        
        public bool IsIncomingRouteAllowed(string routeName)
        {
            return Subscribers
                .Select(x => x.Incoming.Name)
                .Contains(routeName);
        }

        public IEnumerable<Message> CallRoute(Message message)
        {
            return Subscribers
                .Where(x => x.Incoming.Name == message.RouteName)
                .Select(x =>
                {
                    var response = x.Method(message);
                    return new Message(x.Outcoming.Name, response);
                });
        }
        
        public SerializedMessage Serialize(Message message)
        {
            var targetType = Routes
                .First(x => x.Name == message.RouteName)
                .DataType;

            var data = Serialization[targetType].Serialize(message.Payload);
            return new SerializedMessage(message.RouteName, data);
        }
        
        public Message Deserialize(SerializedMessage message)
        {
            var targetType = Routes
                .First(x => x.Name == message.RouteName)
                .DataType;
            
            var payload = Serialization[targetType].Deserialize(message.Data, targetType);
            return new Message(message.RouteName, payload);
        }
    }
}