using Moq;
using NetmqRouter.Helpers;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NUnit.Framework;

namespace NetmqRouter.Tests.Helpers
{
    [TestFixture]
    public class SerializerComparerTests
    {
        #region test classes

        internal class ClassA
        {

        }

        internal class ClassB : ClassA
        {

        }

        #endregion

        [Test]
        public void CompareTwoTypeSerializers()
        {
            // arrange
            var serializerMockA = new Mock<ISerializer<ClassA>>();
            var serializerA = Serializer.FromTypeSerializer(serializerMockA.Object);

            var serializerMockB = new Mock<ISerializer<ClassB>>();
            var serializerB = Serializer.FromTypeSerializer(serializerMockB.Object);

            // assert
            var comparer = new SerializerComparer();
            Assert.AreEqual(-1, comparer.Compare(serializerA, serializerB));
            Assert.AreEqual(1, comparer.Compare(serializerB, serializerA));
        }

        [Test]
        public void CompareTwoGeneralSerializers()
        {
            // arrange
            var serializerMockA = new Mock<IGeneralSerializer<ClassA>>();
            var serializerA = Serializer.FromGeneralSerializer(serializerMockA.Object);

            var serializerMockB = new Mock<IGeneralSerializer<ClassB>>();
            var serializerB = Serializer.FromGeneralSerializer(serializerMockB.Object);

            // assert
            var comparer = new SerializerComparer();
            Assert.AreEqual(-1, comparer.Compare(serializerA, serializerB));
            Assert.AreEqual(1, comparer.Compare(serializerB, serializerA));
        }

        [Test]
        public void CompareVariousSerializers()
        {
            // arrange
            var serializerMock1 = new Mock<ISerializer<string>>();
            var serializer1 = Serializer.FromTypeSerializer(serializerMock1.Object);

            var serializerMock2 = new Mock<IGeneralSerializer<string>>();
            var serializer2 = Serializer.FromGeneralSerializer(serializerMock2.Object);

            // assert
            var comparer = new SerializerComparer();
            Assert.AreEqual(1, comparer.Compare(serializer1, serializer2));
            Assert.AreEqual(-1, comparer.Compare(serializer2, serializer1));
        }
    }
}