using System;
using System.Linq;
using NetmqRouter.Attributes;
using NetmqRouter.BusinessLogic;
using NUnit.Framework;

namespace NetmqRouter.Tests.BusinessLogic
{
    [TestFixture]
    public class ClassAnalyzerTests
    {
        #region Helper classes

        public class ExampleObject
        {
            public int Data { get; set; }
        }

        #endregion

        #region HandleRoutesWithNameSetToNullOrEmpty

        public class ExampleSubscriberWithEmptyIncomingRoute
        {
            [Route("")]
            public void IncomingEmpty() { }
        }

        public class ExampleSubscriberWithNullIncomingRoute
        {
            [Route(null)]
            public void IncomingNull() { }
        }

        [TestCase(typeof(ExampleSubscriberWithEmptyIncomingRoute))]
        [TestCase(typeof(ExampleSubscriberWithNullIncomingRoute))]
        public void HandleRoutesWithNameSetToNullOrEmpty(Type subscriberType)
        {
            Assert.Throws<NetmqRouterException>(() =>
            {
                var subscriber = Activator.CreateInstance(subscriberType);
                ClassAnalyzer.AnalyzeClass(subscriber);
            });
        }

        #endregion

        #region HandleResponseRoutesWithNameSetToNullOrEmpty

        public class ExampleSubscriberWithEmptyOutcomingRoute
        {
            [Route("BasicRoute")]
            [ResponseRoute("")]
            public void OutcomingEmpty() { }
        }

        public class ExampleSubscriberWithNullOutcomingRoute
        {
            [Route("BasicRoute")]
            [ResponseRoute(null)]
            public void OutcomingNull() { }
        }

        [TestCase(typeof(ExampleSubscriberWithEmptyOutcomingRoute))]
        [TestCase(typeof(ExampleSubscriberWithNullOutcomingRoute))]
        public void HandleResponseRoutesWithNameSetToNullOrEmpty(Type subscriberType)
        {
            Assert.Throws<NetmqRouterException>(() =>
            {
                var subscriber = Activator.CreateInstance(subscriberType);
                ClassAnalyzer.AnalyzeClass(subscriber);
            });
        }

        #endregion

        #region HandleMethodWithRouteResponseAttributeButwithoutBasicRoute

        public class ExampleSubscriberWithOutcomingRouteButWithoutIcoming
        {
            [ResponseRoute("SomeRoute")]
            public void Outcomingnull() { }
        }

        [Test]
        public void HandleMethodWithRouteResponseAttributeButwithoutBasicRoute()
        {
            var subscriber = new ExampleSubscriberWithOutcomingRouteButWithoutIcoming();
            Assert.Throws<NetmqRouterException>(() => { ClassAnalyzer.AnalyzeClass(subscriber); });
        }

        #endregion

        #region Route Attribute Analysis

        public class ExampleSubscriberWithIncommingRoutes
        {
            public void NormalMethod() { }

            [Route("GetEvent")]
            public void EventSubscriber() { }

            [Route("GetRaw")]
            public void RawSubscriber(byte[] data) { }

            [Route("GetText")]
            public void TextSubscriber(string text) { }

            [Route("GetObject")]
            public void ObjectSubscriber(ExampleObject _object) { }
        }

        [Test]
        public void DiscoverOnlyMethodsWithRouteAttribute()
        {
            var subscriber = new ExampleSubscriberWithIncommingRoutes();
            var incomingRouteNames = ClassAnalyzer
                .AnalyzeClass(subscriber)
                .Select(x => x.Incoming.Name);

            var expected = new[]
            {
                "GetEvent",
                "GetRaw",
                "GetText",
                "GetObject"
            };

            Assert.AreEqual(expected, incomingRouteNames);
        }

        [TestCase("GetEvent", ExpectedResult = typeof(void))]
        [TestCase("GetRaw", ExpectedResult = typeof(byte[]))]
        [TestCase("GetText", ExpectedResult = typeof(string))]
        [TestCase("GetObject", ExpectedResult = typeof(ExampleObject))]
        public Type CorrectlyDiscoverIncomingDataType(string methodName)
        {
            var subscriber = new ExampleSubscriberWithIncommingRoutes();

            return ClassAnalyzer
                .AnalyzeClass(subscriber)
                .First(x => x.Incoming.Name == methodName)
                .Incoming
                .DataType;
        }

        #endregion

        #region ResponseRoute attribute analysis

        public class ExampleSubscriberWithOutcomingRoutes
        {
            public void NormalMethod() { }

            [Route("RouteA")]
            [ResponseRoute("ResponseEvent")]
            public void EventSource() { }

            [Route("RouteB")]
            [ResponseRoute("ResponseRaw")]
            public byte[] RawSource() => new byte[0];

            [Route("RouteC")]
            [ResponseRoute("ResponseText")]
            public string TextSource() => "test";

            [Route("RouteD")]
            [ResponseRoute("ResponseObject")]
            public ExampleObject ObjectSource() => new ExampleObject();
        }

        [Test]
        public void DiscoverOnlyMethodsWithRouteResponseAttribute()
        {
            var subscriber = new ExampleSubscriberWithOutcomingRoutes();
            var outcomingRouteNames = ClassAnalyzer
                .AnalyzeClass(subscriber)
                .Select(x => x.Outcoming.Name)
                .ToList();

            var expected = new[]
            {
                "ResponseEvent",
                "ResponseRaw",
                "ResponseText",
                "ResponseObject"
            };

            Assert.AreEqual(expected, outcomingRouteNames);
        }

        [TestCase("ResponseEvent", ExpectedResult = typeof(void))]
        [TestCase("ResponseRaw", ExpectedResult = typeof(byte[]))]
        [TestCase("ResponseText", ExpectedResult = typeof(string))]
        [TestCase("ResponseObject", ExpectedResult = typeof(ExampleObject))]
        public Type CorrectlyDiscoverOutcomingDataType(string methodName)
        {
            var subscriber = new ExampleSubscriberWithOutcomingRoutes();
            return ClassAnalyzer
                .AnalyzeClass(subscriber)
                .First(x => x.Outcoming.Name == methodName)
                .Outcoming
                .DataType;
        }

        #endregion

        #region CorrectlyHandleRoutesWithoutRouteResponseAttribute

        public class ExampleSubscriberWithVariousResponseConfiguration
        {
            public void NormalMethod() { }

            [Route("RouteWithoutResponse")]
            public void RouteWithoutResponse() { }

            [Route("RouteWithResponse")]
            [ResponseRoute("TargetRoute")]
            public void RouteWithResponse(byte[] data) { }
        }

        [TestCase("RouteWithoutResponse", ExpectedResult = null)]
        [TestCase("RouteWithResponse", ExpectedResult = "TargetRoute")]
        public string CorrectlyHandleRoutesWithoutRouteResponseAttribute(string incomingROuteName)
        {
            var subscriber = new ExampleSubscriberWithVariousResponseConfiguration();

            return ClassAnalyzer
                .AnalyzeClass(subscriber)
                .First(x => x.Incoming.Name == incomingROuteName)
                .Outcoming
                ?.Name;
        }

        #endregion

        #region Check If Subscriber Gets Only Single Argument

        public class ExampleSubscriberRouteWithMoreThanOneArgument
        {
            [Route("RouteWithResponse")]
            [ResponseRoute("TargetRoute")]
            public void RouteWithResponse(byte[] data, string otherData) { }
        }

        [Test]
        public void HandleRouteWithMoreThanOneArgument()
        {
            var subscriber = new ExampleSubscriberRouteWithMoreThanOneArgument();

            Assert.Throws<NetmqRouterException>(() =>
            {
                ClassAnalyzer.AnalyzeClass(subscriber);
            });
        }

        #endregion

        #region Method calling

        public class ExampleSubscriberWithCallHandler
        {
            public string PassedValue { get; set; }

            [Route("RouteA")]
            public void MethodA(string value) => PassedValue = value;

            [Route("RouteB")]
            public void MethodB(string value) => PassedValue = value;
        }

        [TestCase("RouteA", "MessageA")]
        [TestCase("RouteB", "MessageB")]
        public void CorrectlyCallRoute(string routeName, string value)
        {
            // arrange
            var subscriber = new ExampleSubscriberWithCallHandler();
            var routes = ClassAnalyzer.AnalyzeClass(subscriber);

            // act
            routes
                .First(x => x.Incoming.Name == routeName)
                .Method(value);

            // assert
            Assert.AreEqual(value, subscriber.PassedValue);
        }

        #endregion
    }
}
