using System.Collections.Generic;
using System.Linq;
using Moq;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetmqRouter.Serialization;
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

        #region Indexing

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

            var serializerMockB = new Mock<ITypeSerializer<ClassB>>();
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

        #endregion

        #region GetIncomingRouteNames

        [Test]
        public void GetIncomingRouteNamesFromSubscribers()
        {
            // arrange
            var builder = new DataContractBuilder();

            var serializer = new Mock<ITypeSerializer<string>>();
            builder.RegisterSerializer(serializer.Object);

            var routeA = new Route("RouteA", typeof(string));
            var routeB = new Route("RouteB", typeof(string));
            var routeC = new Route("RouteC", typeof(string));
            var routeD = new Route("RouteD", typeof(string));

            builder.RegisterRoute(routeA);
            builder.RegisterRoute(routeB);
            builder.RegisterRoute(routeC);
            builder.RegisterRoute(routeD);

            // act
            builder.RegisterSubscriber(new Subsriber(routeA, routeB, _ => null));
            builder.RegisterSubscriber(new Subsriber(routeC, routeD, _ => null));

            var dataContract = new DataContractManager(builder);

            // assert
            var routeNames = dataContract
                .GetIncomingRouteNames()
                .ToArray();

            var exptectedRouteNames = new[] { routeA.Name, routeC.Name };
            Assert.AreEqual(exptectedRouteNames, routeNames);
        }

        #endregion

        #region SubscriberCalling

        [Test]
        public void CallRoute()
        {
            // arrange
            var routeI = new Route("RouteI", typeof(ClassA));
            var routeV = new Route("RouteV", typeof(void));
            var routeR1 = new Route("routeR1", typeof(ClassB));
            var routeR2 = new Route("routeR2", typeof(ClassB));

            var serializerMockA = new Mock<ITypeSerializer<ClassA>>();
            serializerMockA.Setup(x => x.Serialize(It.IsAny<ClassA>())).Returns(new[] {(byte) 'A'});

            var serializerMockB = new Mock<ITypeSerializer<ClassB>>();
            serializerMockB.Setup(x => x.Serialize(It.IsAny<ClassB>())).Returns(new[] {(byte) 'B'});

            var responseClass1 = new ClassB();
            var responseClass2 = new ClassB();

            var subscriberA = new Subsriber(routeI, routeR1, _ => responseClass1);
            var subscriberB = new Subsriber(routeI, null, _ => null);
            var subscriberC = new Subsriber(routeI, routeR2, _ => responseClass2);

            var routes = new List<Route>
            {
                routeI, routeV, routeR1, routeR2
            };

            var serializers = new List<Serializer>
            {
                Serializer.FromTypeSerializer(serializerMockA.Object),
                Serializer.FromTypeSerializer(serializerMockB.Object)
            };

            var subscribers = new List<Subsriber>
            {
                subscriberA, subscriberB, subscriberC
            };

            var configuration = new Mock<IDataContractAccess>();
            configuration.Setup(x => x.Routes).Returns(routes);
            configuration.Setup(x => x.Serializers).Returns(serializers);
            configuration.Setup(x => x.Subscribers).Returns(subscribers);

            var contract = new DataContractManager(configuration.Object);

            // act
            var response = contract
                .CallRoute(new Message(routeI.Name, new ClassA()))
                .ToList();

            // assert
            Assert.AreEqual(2, response.Count);
            Assert.AreEqual(new Message(routeR1.Name, responseClass1), response[0]);
            Assert.AreEqual(new Message(routeR2.Name, responseClass2), response[1]);
        }

        #endregion
    }
}