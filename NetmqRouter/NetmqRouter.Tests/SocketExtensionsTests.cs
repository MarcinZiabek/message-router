using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NetmqRouter.Tests
{
    [TestFixture]
    public class SocketExtensionsTests
    {
        [TestCase(nameof(ExampleSubscriber.EventSubscriber), ExpectedResult = "Void")]
        [TestCase(nameof(ExampleSubscriber.RawSubscriber), ExpectedResult = "Raw")]
        [TestCase(nameof(ExampleSubscriber.TextSubscriber), ExpectedResult = "Text")]
        [TestCase(nameof(ExampleSubscriber.ObjectSubscriber), ExpectedResult = "Object")]
        public string IncomingRouteNameWithoutBaseRoute(string methodName)
        {
            var _object = new ExampleSubscriber();
            var routes = ClassAnalyzer.AnalyzeClass(_object);
            return routes.First(x => x.Method.Name == methodName).IncomingRouteName;
        }
    }
}
