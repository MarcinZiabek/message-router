using Moq;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetMQ.Sockets;
using NUnit.Framework;

namespace NetmqRouter.Tests
{
    [TestFixture]
    public class DataContractTests
    {
        private readonly IDataContract _dataContract = new DataContract();
        
        [Test]
        public void RegisterSerializerForNewDataType()
        {
            // arrange
            var dataContract = new DataContract();
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
            var dataContract = new DataContract();
            var serializer = new Mock<ISerializer>();
            var serializer2 = new Mock<ISerializer>();
            
            dataContract.RegisterSerializer(typeof(byte[]), serializer.Object);

            // act
            Assert.Throws<NetmqRouterException>(() =>
            {
                dataContract.RegisterSerializer(typeof(byte[]), serializer2.Object);
            });
        }

        [Test]
        public void RegisterRouteWithSupportedTypeSerializer()
        {
            // arrange
            var dataContract = new DataContract();
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
            var dataContract = new DataContract();
            var route = new Route("BasicRoute", typeof(string));
            
            // act
            Assert.Throws<NetmqRouterException>(() =>
            {
                dataContract.RegisterRoute(route);
            });
        }
        
        [Test]
        public void RegisterSubscriberWithSupportedIncomingRoute()
        {
            // arrange
            var dataContract = new DataContract();
            
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
            var dataContract = new DataContract();
            
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
            var dataContract = new DataContract();
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
            var dataContract = new DataContract();
            
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
    }
}