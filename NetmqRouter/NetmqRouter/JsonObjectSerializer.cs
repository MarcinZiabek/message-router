using System;
using System.Text;
using Newtonsoft.Json;

namespace NetmqRouter
{
    public class JsonObjectSerializer : IObjectSerializer
    {
        public byte[] Serialize(object _object)
        {
            var json = JsonConvert.SerializeObject(_object);
            return Encoding.UTF32.GetBytes(json);
        }

        public object Desialize(byte[] data, Type targetType)
        {
            var json = Encoding.ASCII.GetString(data);
            return JsonConvert.DeserializeObject(json, targetType);
        }
    }
}