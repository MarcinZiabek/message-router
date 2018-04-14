using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetmqRouter.Tests
{
    [TestClass]
    public class ClassAnalyzerTest
    {
        [TestMethod]
        public void AnalyzeClass()
        {
            var _object = new ExampleData();
            var actualRoutes = ClassAnalyzer.AnalyzeClass(_object);

            var expectedRoutes = new List<Route>()
            {
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

            Assert.AreEqual(expectedRoutes[0], actualRoutes[0]);
            Assert.AreEqual(expectedRoutes[1], actualRoutes[1]);
            Assert.AreEqual(expectedRoutes[2], actualRoutes[2]);
        }
    }
}
