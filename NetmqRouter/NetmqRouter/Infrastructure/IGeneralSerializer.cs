using System;

namespace NetmqRouter.Infrastructure
{
    /// <summary>
    /// This interface should be used for the creation of custom data serializers.
    /// </summary>
    public interface IGeneralSerializer
    {
        /// <summary>
        /// Serialize a specified object to byte array.
        /// </summary>
        byte[] Serialize(object _object);

        /// <summary>
        /// Deserialize a specified object from byte array to target type.
        /// </summary>
        object Deserialize(byte[] data, Type targetType);
    }
}