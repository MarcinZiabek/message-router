using System;

namespace NetmqRouter
{
    public interface IObjectSerializer
    {
        byte[] Serialize(object _object);
        object Desialize(byte[] data, Type targetType);
    }
}