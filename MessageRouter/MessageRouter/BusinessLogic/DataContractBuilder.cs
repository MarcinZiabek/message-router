using System.Collections.Generic;
using System.Linq;
using MessageRouter.Exceptions;
using MessageRouter.Helpers;
using MessageRouter.Infrastructure;
using MessageRouter.Models;

namespace MessageRouter.BusinessLogic
{
    internal class DataContractBuilder : IDataContractBuilder, IDataContractAccess
    {
        private readonly List<Route> _routes = new List<Route>();
        private readonly List<Subscriber> _subscribers = new List<Subscriber>();
        private readonly List<Serializer> _serializers  = new List<Serializer>();

        public IReadOnlyList<Route> Routes => _routes;
        public IReadOnlyList<Subscriber> Subscribers => _subscribers;
        public IReadOnlyList<Serializer> Serializers => _serializers;

        public void RegisterSerializer<T>(ITypeSerializer<T> typeSerializer)
        {
            var resultSerializer = Serializer.FromTypeSerializer(typeSerializer);
            RegisterSerializer(resultSerializer);
        }

        public void RegisterGeneralSerializer<T>(IGeneralSerializer<T> serializer)
        {
            var resultSerializer = Serializer.FromGeneralSerializer(serializer);
            RegisterSerializer(resultSerializer);
        }

        private void RegisterSerializer(Serializer serializer)
        {
            if(_serializers.Any(x => x.TargetType == serializer.TargetType))
                throw new ConfigurationException($"Serializer for type {serializer.TargetType} is already registered!");

            _serializers.Add(serializer);
        }

        public void RegisterRoute(Route route)
        {
            if(!_serializers.Any(x => route.DataType.IsSameOrSubclass(x.TargetType)))
                throw new ConfigurationException($"Can not register route with type {route.DataType} because there is no typeSerializer for it.");

            if(_routes.Any(x => x.Name == route.Name))
                throw new ConfigurationException($"Route with name {route.Name} is already registered.");

            _routes.Add(route);
        }

        public void RegisterSubscriber(Subscriber subscriber)
        {
            if(!_routes.Contains(subscriber.Incoming))
                throw new ConfigurationException($"Subscriber refers to not existing route (incoming) type and thereferore can not be registered.");

            if(subscriber.Outcoming != null && !_routes.Contains(subscriber.Outcoming))
                throw new ConfigurationException($"Subscriber refers to not existing route type (outcoming) and thereferore can not be registered.");

            _subscribers.Add(subscriber);
        }
    }
}