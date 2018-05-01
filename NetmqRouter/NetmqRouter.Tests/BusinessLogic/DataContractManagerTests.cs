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

        }
    }
}