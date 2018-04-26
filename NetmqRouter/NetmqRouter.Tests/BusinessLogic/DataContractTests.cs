using Moq;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NUnit.Framework;

namespace NetmqRouter.Tests
{
    [TestFixture]
    public class DataContractTests
    {
        private readonly IDataContract _dataContract = new DataContract();
        
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
    }
}