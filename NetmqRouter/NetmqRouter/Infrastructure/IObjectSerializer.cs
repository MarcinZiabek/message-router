using System;

namespace NetmqRouter.Infrastructure
{
    public interface IObjectSerializer
    {
        byte[] Serialize(object _object);
        object Desialize(byte[] data, Type targetType);
    }
}