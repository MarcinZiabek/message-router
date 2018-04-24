using System.Text;
using NUnit.Framework;

namespace NetmqRouter.Tests.Serialization
{
    [TestFixture]
    public class JsonObjectSerializerTests
    {
        /// <summary>
        /// This class is used only for testing purposes.
        /// </summary>
        private class SimpleObject
        {
            public string Value { get; set; }

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
        
        [Test]
        public void Serialize()
        {
            // arrange
            var serializer = new JsonObjectSerializer(Encoding.ASCII);
            var _object = new SimpleObject("test");
            var expectedResult = Encoding.ASCII.GetBytes("{\"Value\":\"test\"}");
            
            // act
            var serializedData = serializer.Serialize(_object);
            
            // assert
            Assert.AreEqual(expectedResult, serializedData);
        }
        
        [Test]
        public void Deserialize()
        {
            // arrange
            var serializer = new JsonObjectSerializer(Encoding.ASCII);
            var serializedText = Encoding.ASCII.GetBytes("{\"Value\":\"other\"}");
            var expectedResult = new SimpleObject("other");
            
            // act
            var _object = serializer.Deserialize(serializedText, typeof(SimpleObject));
            
            // assert
            Assert.AreEqual(expectedResult, _object);
        }
    }
}