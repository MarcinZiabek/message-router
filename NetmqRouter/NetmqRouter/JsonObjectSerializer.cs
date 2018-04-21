using System;
using System.Text;
using NetmqRouter.Infrastructure;
using Newtonsoft.Json;

namespace NetmqRouter
{
    public class JsonObjectSerializer : IObjectSerializer
    {
        private Encoding _encoding;
        
        public JsonObjectSerializer(Encoding encoding)
        {
            _encoding = encoding;
        }
        
        public JsonObjectSerializer() : this(Encoding.UTF8)
        {
            
        }
        
        public byte[] Serialize(object _object)
        {
            var json = JsonConvert.SerializeObject(_object);
            return _encoding.GetBytes(json);
        }

        public object Desialize(byte[] data, Type targetType)
        {
            var json = _encoding.GetString(data);
            return JsonConvert.DeserializeObject(json, targetType);
        }
    }
}