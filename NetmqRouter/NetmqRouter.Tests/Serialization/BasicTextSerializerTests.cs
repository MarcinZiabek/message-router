using System.Text;
using NetmqRouter.Serialization;
using NUnit.Framework;

namespace NetmqRouter.Tests.Serialization
{
    [TestFixture]
    public class BasicTextSerializerTests
    {
        [Test]
        public void Serialize()
        {
            // arrange
            var serializer = new BasicTextSerializer(Encoding.ASCII);
            var text = "test";
            var expectedResult = new byte[] { 116, 101, 115, 116 };

            // act
            var serializedData = serializer.Serialize(text);

            // assert
            Assert.AreEqual(expectedResult, serializedData);
        }

        [Test]
        public void Deserialize()
        {
            // arrange
            var serializer = new BasicTextSerializer(Encoding.ASCII);
            var serializedText = new byte[] { 116, 101, 115, 116 };
            var expectedResult = "test";

            // act
            var serializedData = serializer.Deserialize(serializedText);

            // assert
            Assert.AreEqual(expectedResult, serializedData);
        }
    }
}