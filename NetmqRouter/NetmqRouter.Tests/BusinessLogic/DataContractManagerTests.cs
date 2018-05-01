using System.Collections.Generic;
using Moq;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NUnit.Framework;

namespace NetmqRouter.Tests.BusinessLogic
{
    [TestFixture]
    public class DataContractManagerTests
    {
        #region Test Classes

        internal class ClassA
        {

        }

        internal class ClassB : ClassA
        {

        }

        internal class ClassC : ClassB
        {

        }

        #endregion

        [Test]
        public void IndexRoutes()
        {
            // arrange
            var routeA = new Route("RouteA", typeof(string));
            var routeB = new Route("RouteB", typeof(int));
            var routeC = new Route("RouteC", typeof(void));

            var routes = new List<Route>()
            {
                routeA, routeB, routeC
            };

            var contract = new Mock<IDataContractAccess>();
            contract.Setup(x => x.Routes).Returns(routes);

            // act
            var routesDictionary = DataContractManager.IndexRoutes(contract.Object);

            // assert
            Assert.AreEqual(new[] { routeA.Name, routeB.Name, routeC.Name }, routesDictionary.Keys);

            Assert.AreEqual(routeA, routesDictionary[routeA.Name]);
            Assert.AreEqual(routeB, routesDictionary[routeB.Name]);
            Assert.AreEqual(routeC, routesDictionary[routeC.Name]);
        }

        [Test]
        public void IndexSubscribers()
        {
            // arrange
            var routeA = new Route("RouteA", typeof(string));
            var routeB = new Route("RouteB", typeof(int));
            var routeC = new Route("RouteC", typeof(void));

            var subscriber1 = new Subsriber(routeA, null, _ => null);
            var subscriber2 = new Subsriber(routeA, routeB, _ => null);
            var subscriber3 = new Subsriber(routeC, null, _ => null);
            var subscriber4 = new Subsriber(routeC, routeB, _ => null);

            var subscribers = new List<Subsriber>()
            {
                subscriber1, subscriber2, subscriber3, subscriber4
            };

            var contract = new Mock<IDataContractAccess>();
            contract.Setup(x => x.Subscribers).Returns(subscribers);

            // act
            var subscribersDictionary = DataContractManager.IndexSubscribers(contract.Object);

            // assert
            Assert.AreEqual(new[] { routeA.Name, routeC.Name }, subscribersDictionary.Keys);

            Assert.AreEqual(new[] { subscriber1, subscriber2 }, subscribersDictionary[routeA.Name]);
            Assert.AreEqual(new[] { subscriber3, subscriber4 }, subscribersDictionary[routeC.Name]);
        }

        [Test]
        public void IndexSerializers()
        {
            // arrange
            var routeA = new Route("RouteA", typeof(ClassA));
            var routeB = new Route("RouteB", typeof(ClassB));
            var routeC = new Route("RouteC", typeof(ClassC));
            var routeG = new Route("RouteG", typeof(object));

            var serializerMockA = new Mock<IGeneralSerializer<ClassA>>();
            serializerMockA.Setup(x => x.Serialize(It.IsAny<ClassA>())).Returns(new[] {(byte) 'A'});

            var serializerMockB = new Mock<ISerializer<ClassB>>();
            serializerMockB.Setup(x => x.Serialize(It.IsAny<ClassB>())).Returns(new[] {(byte) 'B'});

            var serializerMockG = new Mock<IGeneralSerializer<object>>();
            serializerMockG.Setup(x => x.Serialize(It.IsAny<object>())).Returns(new[] {(byte) 'G'});

            var serializerA = Serializer.FromGeneralSerializer(serializerMockA.Object);
            var serializerB = Serializer.FromTypeSerializer(serializerMockB.Object);
            var serializerG = Serializer.FromGeneralSerializer(serializerMockG.Object);

            var routes = new List<Route>()
            {
                routeA, routeB, routeC, routeG
            };

            var serializers = new List<Serializer>()
            {
                serializerA, serializerB, serializerG
            };

            var contract = new Mock<IDataContractAccess>();
            contract.Setup(x => x.Routes).Returns(routes);
            contract.Setup(x => x.Serializers).Returns(serializers);

            // act
            var mapping = DataContractManager.IndexSerializers(contract.Object);

            // assert
            Assert.AreEqual((byte)'A', mapping[routeA.DataType].Serialize(null)[0]);
            Assert.AreEqual((byte)'B', mapping[routeB.DataType].Serialize(null)[0]);
            Assert.AreEqual((byte)'A', mapping[routeC.DataType].Serialize(null)[0]);
            Assert.AreEqual((byte)'G', mapping[routeG.DataType].Serialize(null)[0]);
        }
    }
}