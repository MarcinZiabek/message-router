using NetmqRouter.Infrastructure;

namespace NetmqRouter.Serialization
{
    /// <summary>
    /// Basic serializer that pass byte arrays "as is".
    /// </summary>
    public class RawDataSerializer : ISerializer<byte[]>
    {
        public byte[] Serialize(byte[] _object) => _object;

        public byte[] Deserialize(byte[] data) => data;
    }
}