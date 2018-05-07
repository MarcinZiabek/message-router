using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NetmqRouter.Infrastructure;
using Newtonsoft.Json;

namespace NetmqRouter.Serialization
{
    /// <summary>
    /// This class can be used for serialization of any object to the XML format.
    /// </summary>
    public class XmlObjectSerializer : IGeneralSerializer<object>
    {
        private readonly Encoding _encoding;

        /// <param name="encoding">Encoding that will be used for text serialization.</param>
        public XmlObjectSerializer(Encoding encoding)
        {
            _encoding = encoding;
        }

        public XmlObjectSerializer() : this(Encoding.UTF8)
        {

        }

        public byte[] Serialize(object _object)
        {
            var serializer = new XmlSerializer(_object.GetType());

            using (var textWriter = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings { Indent = false }))
            {
                serializer.Serialize(xmlWriter, _object);
                return _encoding.GetBytes(textWriter.ToString());
            }
        }

        public object Deserialize(byte[] data, Type targetType)
        {
            var serializer = new XmlSerializer(targetType);
            var json = _encoding.GetString(data);

            using (var reader = new StringReader(json))
                return serializer.Deserialize(reader);
        }
    }
}