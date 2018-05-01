using NetmqRouter.Infrastructure;

namespace NetmqRouter.Serialization
{
    public class VoidSerializer : ITypeSerializer<object>
    {
        public byte[] Serialize(object _object)
        {
            return null;
        }

        public object Deserialize(byte[] data)
        {
            return null;
        }
    }
}