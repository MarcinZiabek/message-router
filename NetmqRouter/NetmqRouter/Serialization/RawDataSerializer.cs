using System;
using NetmqRouter.Infrastructure;

namespace NetmqRouter
{
    public class RawDataSerializer : ISerializer
    {
        public byte[] Serialize(object _object)
        {
            return (byte[])_object;
        }

        public object Deserialize(byte[] data, Type targetType)
        {
            return data;
        }
    }
}