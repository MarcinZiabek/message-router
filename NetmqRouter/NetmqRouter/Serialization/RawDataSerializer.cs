using System;
using NetmqRouter.Infrastructure;

namespace NetmqRouter
{
    /// <summary>
    /// Basic serializer that pass byte arrays "as is".
    /// </summary>
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