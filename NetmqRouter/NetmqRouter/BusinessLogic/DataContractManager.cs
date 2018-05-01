using System;
using System.Collections.Generic;
using System.Linq;
using NetmqRouter.Helpers;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.BusinessLogic
{
    internal class DataContractManager : IDataContractOperations
    {
        private readonly IReadOnlyDictionary<string, Route> _routes;
        private readonly IReadOnlyDictionary<string, List<Subsriber>> _subscribers;
        private readonly IReadOnlyDictionary<Type, Serializer> _serializers;

        public DataContractManager(IDataContractAccess dataContract)
        {
            _routes = IndexRoutes(dataContract);
            _subscribers = IndexSubscribers(dataContract);
            _serializers = IndexSerializers(dataContract);
        }

        internal static IReadOnlyDictionary<string, Route> IndexRoutes(IDataContractAccess dataContract)
        {
            return dataContract
                .Routes
                .ToDictionary(
                    x => x.Name,
                    x => x);
        }

        internal static IReadOnlyDictionary<string, List<Subsriber>> IndexSubscribers(IDataContractAccess dataContract)
        {
            return dataContract
                .Subscribers
                .GroupBy(x => x.Incoming.Name)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList());
        }

        internal static IReadOnlyDictionary<Type, Serializer> IndexSerializers(IDataContractAccess dataContract)
        {
            var sortedSerializers = dataContract
                .Serializers
                .OrderByDescending(x => x, new SerializerComparer());

            return dataContract
                .Routes
                .Select(x => x.DataType)
                .Distinct()
                .ToDictionary(
                    x => x,
                    x => FindSerializer(sortedSerializers, x));

            Serializer FindSerializer(IOrderedEnumerable<Serializer> serializers, Type targetType)
            {
                var serializer = serializers.First(y => targetType.IsSameOrSubclass(y.TargetType));
                return serializer.IsGeneral ? serializer.ToTypeSerializer(targetType) : serializer;
            }
        }

        public IEnumerable<string> GetIncomingRouteNames()
        {
            return _subscribers
                .SelectMany(x => x.Value)
                .Select(x => x.Incoming.Name);
        }

        public IEnumerable<Message> CallRoute(Message message)
        {
            return _subscribers
                [message.RouteName]
                .Select(x =>
                {
                    var response = x.Method(message.Payload);
                    return (x.Outcoming == null) ? null : new Message(x.Outcoming.Name, response);
                })
                .Where(x => x != null);
        }

        private Serializer GetSerializer(string routeName)
        {
            var targetType = _routes[routeName].DataType;
            var serializer = _serializers[targetType];

            return serializer;
        }

        public SerializedMessage Serialize(Message message)
        {
            var serializer = GetSerializer(message.RouteName);
            var data = serializer.Serialize(message.Payload);
            return new SerializedMessage(message.RouteName, data);
        }

        public Message Deserialize(SerializedMessage message)
        {
            var serializer = GetSerializer(message.RouteName);
            var payload = serializer.Deserialize(message.Data);
            return new Message(message.RouteName, payload);
        }
    }
}