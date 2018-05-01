namespace NetmqRouter.Infrastructure
{
    /// <summary>
    /// This interface should be used for the creation of custom data serializers.
    /// </summary>
    public interface ISerializer<T>
    {
        /// <summary>
        /// Serialize a specified object to byte array.
        /// </summary>
        byte[] Serialize(T _object);

        /// <summary>
        /// Deserialize a specified object from byte array to target type.
        /// </summary>
        T Deserialize(byte[] data);
    }
}