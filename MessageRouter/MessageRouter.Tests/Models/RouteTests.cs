using MessageRouter.Models;
using NUnit.Framework;

namespace NetmqRouter.Tests.Models
{
    [TestFixture]
    public class RouteTests
    {
        [Test]
        public void RouteToString()
        {
            // arrange
            var route = new Route("TestName", typeof(string));
            
            // act
            var name = route.ToString();
            
            // assert
            Assert.AreEqual("Route(TestName, String)", name);
        }
        
        [Test]
        public void EventToString()
        {
            // arrange
            var route = new Route("AnotherName");
            
            // act
            var name = route.ToString();
            
            // assert
            Assert.AreEqual("Route(AnotherName, Event)", name);
        }
    }
}