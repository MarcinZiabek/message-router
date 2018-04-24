using NUnit.Framework;

namespace NetmqRouter.Tests.Serialization
{
    [TestFixture]
    public class RawDataSerializerTests
    {
        [Test]
        public void Serialize()
        {
            // arrange
            var serializer = new RawDataSerializer();
            var data = new byte[] {1, 2, 3};
            
            // act
            var serializedData = serializer.Serialize(data);
            
            // assert
            Assert.AreEqual(data, serializedData);
        }
        
        [Test]
        public void Deserialize()
        {
            // arrange
            var serializer = new RawDataSerializer();
            var serializedData = new byte[] {1, 2, 3};
            
            // act
            var data = serializer.Deserialize(serializedData, typeof(byte[]));
            
            // assert
            Assert.AreEqual(serializedData, data);
        }
    }
}