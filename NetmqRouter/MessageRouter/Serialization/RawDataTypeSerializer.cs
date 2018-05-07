using NetmqRouter.Infrastructure;

namespace NetmqRouter.Serialization
{
    /// <summary>
    /// Basic typeSerializer that pass byte arrays "as is".
    /// </summary>
    public class RawDataTypeSerializer : ITypeSerializer<byte[]>
    {
        public byte[] Serialize(byte[] _object) => _object;

        public byte[] Deserialize(byte[] data) => data;
    }
}