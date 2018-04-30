using System;
using System.Collections;
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
        private readonly List<Subsriber> _subscribers = new List<Subsriber>();
        private readonly List<Serializer> _serializers  = new List<Serializer>();

        public IReadOnlyList<Route> Routes => _routes;
        public IReadOnlyList<Subsriber> Subscribers => _subscribers;
        public IReadOnlyList<Serializer> Serializers => _serializers;

        public void RegisterSerializer<T>(ISerializer<T> serializer)
        {
            var resultSerializer = Serializer.FromTypeSerializer(serializer);
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
                throw new NetmqRouterException($"Serializer for type {serializer.TargetType} is already registered!");

            _serializers.Add(serializer);
        }

        public void RegisterRoute(Route route)
        {
            if(!_serializers.Any(x => route.DataType.IsSameOrSubclass(x.TargetType)))
                throw new NetmqRouterException($"Can not register route with type {route.DataType} because there is no serializer for it.");

            if(_routes.Any(x => x.Name == route.Name))
                throw new NetmqRouterException($"Route with name {route.Name} is already registered.");

            _routes.Add(route);
        }

        public void RegisterSubscriber(Subsriber subsriber)
        {
            if(!_routes.Contains(subsriber.Incoming))
                throw new NetmqRouterException($"Subscriber refers to not existing route (incoming) type and thereferore can not be registered.");

            if(subsriber.Outcoming != null && !_routes.Contains(subsriber.Outcoming))
                throw new NetmqRouterException($"Subscriber refers to not existing route type (outcoming) and thereferore can not be registered.");

            _subscribers.Add(subsriber);
        }
    }
}