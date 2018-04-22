using System;

namespace NetmqRouter.Infrastructure
{
    public interface ISerializer
    {
        byte[] Serialize(object _object);
        object Deserialize(byte[] data, Type targetType);
    }
}