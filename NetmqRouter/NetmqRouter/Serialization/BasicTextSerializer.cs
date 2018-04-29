using System;
using System.Text;
using NetmqRouter.Infrastructure;

namespace NetmqRouter.Serialization
{
    /// <summary>
    /// This class can be used for text serialization.
    /// </summary>
    public class BasicTextSerializer : ISerializer
    {
        private readonly Encoding _encoding;
        
        /// <param name="encoding">Encoding that will be used for text serialization.</param>
        public BasicTextSerializer(Encoding encoding)
        {
            _encoding = encoding;
        }
        
        public BasicTextSerializer() : this(Encoding.UTF8)
        {
            
        }
        
        public byte[] Serialize(object _object)
        {
            return _encoding.GetBytes((string)_object);
        }

        public object Deserialize(byte[] data, Type targetType)
        {
            return _encoding.GetString(data);
        }
    }
}