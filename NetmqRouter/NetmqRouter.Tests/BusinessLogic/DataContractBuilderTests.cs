using System.Linq;
using Moq;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NUnit.Framework;

namespace NetmqRouter.Tests.BusinessLogic
{
    [TestFixture]
    public class DataContractBuilderTests
    {
        #region Registering Serializer

        [Test]
        public void RegisterSerializerForNewDataType()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var serializer = new Mock<ISerializer>();

            // act
            Assert.DoesNotThrow(() =>
            {
                dataContract.RegisterSerializer(typeof(byte[]), serializer.Object);
            });
        }

        [Test]
        public void RegisterSecondSerializerForTheSameType()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var serializer = new Mock<ISerializer>();
            var serializer2 = new Mock<ISerializer>();

            dataContract.RegisterSerializer(typeof(byte[]), serializer.Object);

            // act
            Assert.Throws<NetmqRouterException>(() =>
            {
                dataContract.RegisterSerializer(typeof(byte[]), serializer2.Object);
            });
        }

        #endregion

        #region Registering Route

        [Test]
        public void RegisterRouteWithSupportedTypeSerializer()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var serializer = new Mock<ISerializer>();
            var route = new Route("BasicRoute", typeof(string));

            dataContract.RegisterSerializer(typeof(string), serializer.Object);

            // act
            Assert.DoesNotThrow(() =>
            {
                dataContract.RegisterRoute(route);
            });
        }

        [Test]
        public void RegisterRouteWithNotSupportedTypeSerializer()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var route = new Route("BasicRoute", typeof(string));

            // act
            Assert.Throws<NetmqRouterException>(() =>
            {
                dataContract.RegisterRoute(route);
            });
        }

        [Test]
        public void RegisterTwoRoutesWithTheSameName()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            dataContract.RegisterSerializer(typeof(string), new Mock<ISerializer>().Object);
            dataContract.RegisterSerializer(typeof(int), new Mock<ISerializer>().Object);

            // act
            dataContract.RegisterRoute(new Route("BasicRoute", typeof(string)));

            Assert.Throws<NetmqRouterException>(() =>
            {
                dataContract.RegisterRoute(new Route("BasicRoute", typeof(int)));
            });
        }

        #endregion

        #region Registering Subscriber

        [Test]
        public void RegisterSubscriberWithSupportedIncomingRoute()
        {
            // arrange
            var dataContract = new DataContractBuilder();

            var serializer = new Mock<ISerializer>();
            var route = new Route("RouteA", typeof(string));
            var subscriber = new RouteSubsriber(route, null, _ => null);

            dataContract.RegisterSerializer(typeof(string), serializer.Object);
            dataContract.RegisterRoute(route);

            // act
            Assert.DoesNotThrow(() =>
            {
                dataContract.RegisterSubscriber(subscriber);
            });
        }

        [Test]
        public void RegisterSubscriberWithSupportedBothRoutes()
        {
            // arrange
            var dataContract = new DataContractBuilder();

            var serializer = new Mock<ISerializer>();
            var incomingRoute = new Route("IncomingRoute", typeof(string));
            var outcomingRoute = new Route("OutcomingRoute", typeof(string));
            var subscriber = new RouteSubsriber(incomingRoute, outcomingRoute, _ => null);

            dataContract.RegisterSerializer(typeof(string), serializer.Object);
            dataContract.RegisterRoute(incomingRoute);
            dataContract.RegisterRoute(outcomingRoute);

            // act
            Assert.DoesNotThrow(() =>
            {
                dataContract.RegisterSubscriber(subscriber);
            });
        }

        [Test]
        public void RegisterSubscriberWithNotSupportedIncomingRoute()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var route = new Route("BasicRoute", typeof(string));
            var subscriber = new RouteSubsriber(route, null, _ => null);

            // act
            Assert.Throws<NetmqRouterException>(() =>
            {
                dataContract.RegisterSubscriber(subscriber);
            });
        }

        [Test]
        public void RegisterSubscriberWithNotSupportedOutcomingRoute()
        {
            // arrange
            var dataContract = new DataContractBuilder();

            var serializer = new Mock<ISerializer>();
            var incomingRoute = new Route("IncomingRoute", typeof(string));
            var outcomingRoute = new Route("OutcomingRoute", typeof(string));
            var subscriber = new RouteSubsriber(incomingRoute, outcomingRoute, _ => null);

            dataContract.RegisterSerializer(typeof(string), serializer.Object);
            dataContract.RegisterRoute(incomingRoute);

            // act
            Assert.Throws<NetmqRouterException>(() =>
            {
                dataContract.RegisterSubscriber(subscriber);
            });
        }

        #endregion
    }
}