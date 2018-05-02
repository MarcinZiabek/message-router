using System.Text;
using NetmqRouter.Infrastructure;

namespace NetmqRouter.Serialization
{
    /// <summary>
    /// This class can be used for text serialization.
    /// </summary>
    public class BasicTextTypeSerializer : ITypeSerializer<string>
    {
        private readonly Encoding _encoding;

        /// <param name="encoding">Encoding that will be used for text serialization.</param>
        public BasicTextTypeSerializer(Encoding encoding)
        {
            _encoding = encoding;
        }

        public BasicTextTypeSerializer() : this(Encoding.UTF8)
        {

        }

        public byte[] Serialize(string text) => _encoding.GetBytes(text);
        public string Deserialize(byte[] data) => _encoding.GetString(data);
    }
}