using System;
using System.Text;
using NetmqRouter.Attributes;
using NetmqRouter.Infrastructure;

namespace NetmqRouter
{
    public class BasicTextSerializer : ISerializer
    {
        private Encoding _encoding;
        
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