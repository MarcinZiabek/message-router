using System.Runtime.CompilerServices;
using Moq;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Exceptions;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NUnit.Framework;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

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
            var serializer = new Mock<ITypeSerializer<byte[]>>();

            // act
            Assert.DoesNotThrow(() =>
            {
                dataContract.RegisterSerializer(serializer.Object);
            });
        }

        [Test]
        public void RegisterSecondSerializerForTheSameType()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var serializer = new Mock<ITypeSerializer<string>>();
            var serializer2 = new Mock<ITypeSerializer<string>>();

            dataContract.RegisterSerializer(serializer.Object);

            // act
            Assert.Throws<ConfigurationException>(() =>
            {
                dataContract.RegisterSerializer(serializer2.Object);
            });
        }

        #endregion

        #region Register General Serializer

        internal class ClassA
        {

        }

        internal class ClassB : ClassA
        {

        }

        internal class ClassC : ClassB
        {

        }

        [Test]
        public void RegisterGeneralRoute()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var serializer = new Mock<IGeneralSerializer<ClassB>>();

            // act
            Assert.DoesNotThrow(() =>
            {
                dataContract.RegisterGeneralSerializer(serializer.Object);
            });
        }

        [Test]
        public void RegisterGeneralSecondSerializerForTheSameType()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var serializer = new Mock<IGeneralSerializer<ClassB>>();
            var serializer2 = new Mock<IGeneralSerializer<ClassB>>();

            dataContract.RegisterGeneralSerializer(serializer.Object);

            // act
            Assert.Throws<ConfigurationException>(() =>
            {
                dataContract.RegisterGeneralSerializer(serializer2.Object);
            });
        }

        [Test]
        public void RegisterGeneralRouteForSubclassType()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var serializer = new Mock<IGeneralSerializer<ClassA>>();
            var serializer2 = new Mock<IGeneralSerializer<ClassB>>();

            dataContract.RegisterGeneralSerializer(serializer.Object);

            // act
            Assert.DoesNotThrow(() =>
            {
                dataContract.RegisterGeneralSerializer(serializer2.Object);
            });
        }

        [Test]
        public void RegisterGeneralRouteForDerivedType()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var serializer = new Mock<IGeneralSerializer<ClassB>>();
            var serializer2 = new Mock<IGeneralSerializer<ClassC>>();

            dataContract.RegisterGeneralSerializer(serializer.Object);

            // act
            Assert.DoesNotThrow(() =>
            {
                dataContract.RegisterGeneralSerializer(serializer2.Object);
            });
        }

        #endregion

        #region Registering Route

        [Test]
        public void RegisterRouteWithSupportedTypeSerializer()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            var serializer = new Mock<ITypeSerializer<string>>();
            var route = new Route("BasicRoute", typeof(string));

            dataContract.RegisterSerializer(serializer.Object);

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
            Assert.Throws<ConfigurationException>(() =>
            {
                dataContract.RegisterRoute(route);
            });
        }

        [Test]
        public void RegisterTwoRoutesWithTheSameName()
        {
            // arrange
            var dataContract = new DataContractBuilder();
            dataContract.RegisterSerializer(new Mock<ITypeSerializer<string>>().Object);
            dataContract.RegisterSerializer(new Mock<ITypeSerializer<int>>().Object);

            // act
            dataContract.RegisterRoute(new Route("BasicRoute", typeof(string)));

            Assert.Throws<ConfigurationException>(() =>
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

            var serializer = new Mock<ITypeSerializer<string>>();
            var route = new Route("RouteA", typeof(string));
            var subscriber = new Subsriber(route, null, _ => null);

            dataContract.RegisterSerializer(serializer.Object);
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

            var serializer = new Mock<ITypeSerializer<string>>();
            var incomingRoute = new Route("IncomingRoute", typeof(string));
            var outcomingRoute = new Route("OutcomingRoute", typeof(string));
            var subscriber = new Subsriber(incomingRoute, outcomingRoute, _ => null);

            dataContract.RegisterSerializer(serializer.Object);
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
            var subscriber = new Subsriber(route, null, _ => null);

            // act
            Assert.Throws<ConfigurationException>(() =>
            {
                dataContract.RegisterSubscriber(subscriber);
            });
        }

        [Test]
        public void RegisterSubscriberWithNotSupportedOutcomingRoute()
        {
            // arrange
            var dataContract = new DataContractBuilder();

            var serializer = new Mock<ITypeSerializer<string>>();
            var incomingRoute = new Route("IncomingRoute", typeof(string));
            var outcomingRoute = new Route("OutcomingRoute", typeof(string));
            var subscriber = new Subsriber(incomingRoute, outcomingRoute, _ => null);

            dataContract.RegisterSerializer(serializer.Object);
            dataContract.RegisterRoute(incomingRoute);

            // act
            Assert.Throws<ConfigurationException>(() =>
            {
                dataContract.RegisterSubscriber(subscriber);
            });
        }

        #endregion
    }
}