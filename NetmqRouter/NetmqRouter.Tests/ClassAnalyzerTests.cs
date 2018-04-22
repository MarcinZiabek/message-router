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

        [TestCase(nameof(ExampleSubscriber.EventSubscriber), ExpectedResult = "Example/Void")]
        [TestCase(nameof(ExampleSubscriber.RawSubscriber), ExpectedResult = "Example/Raw")]
        [TestCase(nameof(ExampleSubscriber.TextSubscriber), ExpectedResult = "Example/Text")]
        [TestCase(nameof(ExampleSubscriber.ObjectSubscriber), ExpectedResult = "Example/Object")]
        public string IncomingRouteNameWithBaseRoute(string methodName)
        {
            var _object = new ExampleSubscriberWithBaseRoute();
            var routes = ClassAnalyzer.AnalyzeClass(_object);
            return routes.First(x => x.Method.Name == methodName).IncomingRouteName;
        }

        [TestCase(nameof(ExampleSubscriber.EventSubscriber), ExpectedResult = "Response")]
        [TestCase(nameof(ExampleSubscriber.RawSubscriber), ExpectedResult = "Response")]
        [TestCase(nameof(ExampleSubscriber.TextSubscriber), ExpectedResult = null)]
        [TestCase(nameof(ExampleSubscriber.ObjectSubscriber), ExpectedResult = null)]
        public string OutcomingRouteNameWithBaseRoute(string methodName)
        {
            var _object = new ExampleSubscriber();
            var routes = ClassAnalyzer.AnalyzeClass(_object);
            return routes.First(x => x.Method.Name == methodName).OutcomingRouteName;
        }

        [TestCase(nameof(ExampleSubscriber.EventSubscriber), ExpectedResult = null)]
        [TestCase(nameof(ExampleSubscriber.RawSubscriber), ExpectedResult = typeof(byte[]))]
        [TestCase(nameof(ExampleSubscriber.TextSubscriber), ExpectedResult = typeof(string))]
        [TestCase(nameof(ExampleSubscriber.ObjectSubscriber), ExpectedResult = typeof(SimpleObject))]
        public Type IncomingRouteDataType(string methodName)
        {
            var _object = new ExampleSubscriber();
            var routes = ClassAnalyzer.AnalyzeClass(_object);
            return routes.First(x => x.Method.Name == methodName).IncomingDataType;
        }

        [TestCase(nameof(ExampleSubscriber.EventSubscriber), ExpectedResult = typeof(byte[]))]
        [TestCase(nameof(ExampleSubscriber.RawSubscriber), ExpectedResult = typeof(string))]
        [TestCase(nameof(ExampleSubscriber.TextSubscriber), ExpectedResult = typeof(SimpleObject))]
        [TestCase(nameof(ExampleSubscriber.ObjectSubscriber), ExpectedResult = null)]
        public Type OutcomingRouteDataType(string methodName)
        {
            var _object = new ExampleSubscriber();
            var routes = ClassAnalyzer.AnalyzeClass(_object);
            return routes.First(x => x.Method.Name == methodName).OutcomingDataType;
        }

        [TestCase(nameof(ExampleSubscriber.EventSubscriber), ExpectedResult = nameof(ExampleSubscriber.EventSubscriber))]
        [TestCase(nameof(ExampleSubscriber.RawSubscriber), ExpectedResult = nameof(ExampleSubscriber.RawSubscriber))]
        [TestCase(nameof(ExampleSubscriber.TextSubscriber), ExpectedResult = nameof(ExampleSubscriber.TextSubscriber))]
        [TestCase(nameof(ExampleSubscriber.ObjectSubscriber), ExpectedResult = nameof(ExampleSubscriber.ObjectSubscriber))]
        public string RoutesMapping(string methodName)
        {
            // arrange
            var _object = new ExampleSubscriber();
            var routes = ClassAnalyzer.AnalyzeClass(_object);

            // act
            routes.First(x => x.Method.Name == methodName).Call(null);

            // assert
            return _object.CalledMethod;
        }
    }
}
