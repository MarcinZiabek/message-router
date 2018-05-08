using System;
using System.Text;
using MessageRouter.Infrastructure;
using Newtonsoft.Json;

namespace MessageRouter.Json
{
    /// <summary>
    /// This class can be used for serialization of any object to the JSON format.
    /// It uses the Newtonsoft.Json library.
    /// </summary>
    public class JsonObjectSerializer : IGeneralSerializer<object>
    {
        private readonly Encoding _encoding;

        /// <param name="encoding">Encoding that will be used for text serialization.</param>
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

        public object Deserialize(byte[] data, Type targetType)
        {
            var json = _encoding.GetString(data);
            return JsonConvert.DeserializeObject(json, targetType);
        }
    }
}