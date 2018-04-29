using System;
using NetmqRouter.Infrastructure;

namespace NetmqRouter.Models
{
    internal class Serializer : ISerializer<object>
    {
        public Type TargetType { get; private set; }
        public bool IsGeneric { get; private set; }

        private Func<object, byte[]> SerializeFunction { get; set; }
        private Func<byte[], object> DeserializeFunction { get; set; }

        public byte[] Serialize(object _object) => SerializeFunction(_object);
        public object Deserialize(byte[] data) => DeserializeFunction(data);

        public static Serializer FromTypeSerializer<T>(ISerializer<T> serializer)
        {
            return new Serializer()
            {
                TargetType = typeof(T),
                IsGeneric = false,
                SerializeFunction = obj => serializer.Serialize((T)obj),
                DeserializeFunction = data => serializer.Deserialize(data)
            };
        }

        public static Serializer FromGeneralSerializer(Type targetType, IGeneralSerializer serializer)
        {
            return new Serializer()
            {
                TargetType = targetType,
                IsGeneric = true,
                SerializeFunction = serializer.Serialize,
                DeserializeFunction = data => serializer.Deserialize(data, targetType)
            };
        }
    }
}