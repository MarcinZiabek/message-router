using System.Linq;
using Moq;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NUnit.Framework;

namespace NetmqRouter.Tests.BusinessLogic
{
    [TestFixture]
    public class DataContractOperationsTests
    {
        private IDataContractOperations _dataContract;

        [SetUp]
        public void SetUp()
        {
            var builder = new DataContractBuilder();
            _dataContract = new DataContractManager(builder);
        }

        #region GetIncomingRouteNames

        [Test]
        public void GetIncomingRouteNamesFromSubscribers()
        {
            // arrange
            var builder = new DataContractBuilder();

            var serializer = new Mock<ISerializer>();
            builder.RegisterSerializer(typeof(string), serializer.Object);

            var routeA = new Route("RouteA", typeof(string));
            var routeB = new Route("RouteB", typeof(string));
            var routeC = new Route("RouteC", typeof(string));
            var routeD = new Route("RouteD", typeof(string));

            builder.RegisterRoute(routeA);
            builder.RegisterRoute(routeB);
            builder.RegisterRoute(routeC);
            builder.RegisterRoute(routeD);

            // act
            var dataContract = new DataContractManager(builder);

            builder.RegisterSubscriber(new  RouteSubsriber(routeA, routeB, _ => null));
            builder.RegisterSubscriber(new  RouteSubsriber(routeC, routeD, _ => null));

            // assert
            var routeNames = dataContract
                .GetIncomingRouteNames()
                .ToArray();

            var exptectedRouteNames = new[] { routeA.Name, routeC.Name };
            Assert.AreEqual(exptectedRouteNames, routeNames);
        }

        #endregion
    }
}