using System;
using System.Text;
using MessageRouter.Serialization;
using NUnit.Framework;

namespace NetmqRouter.Tests.Serialization
{
    public class XmlSerializationTests
    {
        /// <summary>
        /// This class is used only for testing purposes.
        /// </summary>
        [Serializable]
        public class SimpleObject
        {
            public string Value { get; set; }

            public SimpleObject()
            {

            }

            public SimpleObject(string value)
            {
                Value = value;
            }

            public override bool Equals(object obj)
            {
                return obj is SimpleObject o &&
                       this.Value == o.Value;
            }
        }

        private const string XmlDocument = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                                           "<SimpleObject xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                                           "<Value>test</Value>" +
                                           "</SimpleObject>";

        [Test]
        public void Serialize()
        {
            // arrange
            var serializer = new XmlObjectSerializer(Encoding.ASCII);
            var _object = new SimpleObject("test");
            var expectedResult = Encoding.ASCII.GetBytes(XmlDocument);

            // act
            var serializedData = serializer.Serialize(_object);

            // assert
            Assert.AreEqual(expectedResult, serializedData);
        }

        [Test]
        public void Deserialize()
        {
            // arrange
            var serializer = new XmlObjectSerializer(Encoding.ASCII);
            var serializedText = Encoding.ASCII.GetBytes(XmlDocument);
            var expectedResult = new SimpleObject("test");

            // act
            var _object = serializer.Deserialize(serializedText, typeof(SimpleObject));

            // assert
            Assert.AreEqual(expectedResult, _object);
        }
    }
}