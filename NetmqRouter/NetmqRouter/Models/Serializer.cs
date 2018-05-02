using System;
using NetmqRouter.Infrastructure;

namespace NetmqRouter.Models
{
    internal class Serializer
    {
        public Type TargetType { get; private set; }
        public bool IsGeneral { get; private set; }

        private Func<object, byte[]> SerializeFunction { get; set; }
        private Func<byte[], Type, object> DeserializeFunction { get; set; }

        public byte[] Serialize(object _object) => SerializeFunction(_object);
        public object Deserialize(byte[] data) => DeserializeFunction(data, TargetType);

        private Serializer()
        {

        }

        public static Serializer FromTypeSerializer<T>(ITypeSerializer<T> typeSerializer)
        {
            return new Serializer()
            {
                TargetType = typeof(T),
                IsGeneral = false,
                SerializeFunction = obj => typeSerializer.Serialize((T)obj),
                DeserializeFunction = (data, _) => typeSerializer.Deserialize(data)
            };
        }

        public static Serializer FromGeneralSerializer<T>(IGeneralSerializer<T> serializer)
        {
            return new Serializer()
            {
                TargetType = typeof(T),
                IsGeneral = true,
                SerializeFunction = serializer.Serialize,
                DeserializeFunction = serializer.Deserialize
            };
        }

        public Serializer ToTypeSerializer(Type targetType)
        {
            if(!IsGeneral)
                throw new Exception("Only general serializers can be replaced to type ones.");

            return new Serializer()
            {
                TargetType = targetType,
                IsGeneral = false,
                SerializeFunction = Serialize,
                DeserializeFunction = (data, _) => Deserialize(data)
            };
        }
    }
}