using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NetmqRouter.Tests
{
    [TestFixture]
    public class ClassAnalyzerTests
    {
        [Test]
        public void AnalyzeClass()
        {
            // arrange
            var _object = new ExampleData();

            // act
            var actualRoutes = ClassAnalyzer.AnalyzeClass(_object);

            // assert
            var expectedRoutes = new List<Route>()
            {
                new Route
                {
                    Name = "Example/Event",
                    DataType = RouteDataType.Event
                },
                new Route
                {
                    Name = "Example/Raw",
                    DataType = RouteDataType.RawData
                },
                new Route
                {
                    Name = "Example/Text",
                    DataType = RouteDataType.Text
                },
                new Route
                {
                    Name = "Example/Object",
                    DataType = RouteDataType.Object
                }
            };

            Assert.AreEqual(expectedRoutes, actualRoutes);
        }

        [TestCase("Event", ExpectedResult = nameof(ExampleData.EventSubscriber))]
        [TestCase("Raw", ExpectedResult = nameof(ExampleData.RawSubscriber))]
        [TestCase("Text", ExpectedResult = nameof(ExampleData.TextSubscriber))]
        [TestCase("Object", ExpectedResult = nameof(ExampleData.ObjectSubscriber))]
        public string CallRoutes(string routeName)
        {
            // arrange
            var _object = new ExampleData();
            var routes = ClassAnalyzer.AnalyzeClass(_object);

            // act
            routes.First(x => x.Name == "Example/" + routeName).Target(null);

            // assert
            return _object.CalledMethod;
        }

        [TestCase(null, ExpectedResult = RouteDataType.Event)]
        [TestCase(typeof(byte[]), ExpectedResult = RouteDataType.RawData)]
        [TestCase(typeof(string), ExpectedResult = RouteDataType.Text)]
        [TestCase(typeof(SimpleObject), ExpectedResult = RouteDataType.Object)]
        public RouteDataType CovertTypeToRouteDataType(Type type)
        {
            return ClassAnalyzer.CovertTypeToRouteDataType(type);
        }
    }
}
