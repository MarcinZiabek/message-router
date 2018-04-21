using System.Text;
using NetmqRouter.Attributes;
using NetmqRouter.Infrastructure;

namespace NetmqRouter
{
    public class BasicTextSerializer : ITextSerializer
    {
        private Encoding _encoding;
        
        public BasicTextSerializer(Encoding encoding)
        {
            _encoding = encoding;
        }
        
        public BasicTextSerializer() : this(Encoding.UTF8)
        {
            
        }
        
        public byte[] Serialize(string text)
        {
            return _encoding.GetBytes(text);
        }

        public string Desialize(byte[] data)
        {
            return _encoding.GetString(data);
        }
    }
}