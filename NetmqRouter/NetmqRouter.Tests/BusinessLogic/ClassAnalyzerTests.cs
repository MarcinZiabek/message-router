using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using NetmqRouter.Attributes;
using NetmqRouter.BusinessLogic;
using NUnit.Framework;

namespace NetmqRouter.Tests
{
    [TestFixture]
    public class ClassAnalyzerTests
    {
        #region test classes

        public class ExampleObject
        {
            public int Data { get; set; }
        }

        public class ExampleSubscriberWithNullIncomingRoute
        {
            [Route(null)]
            public void IncomingNull() { }
            
            [Route("")]
            public void IncomingEmpty() { }
        }
        
        public class ExampleSubscriberWithNullOutcomingRoute
        {
            [Route("BasicRoute")]
            [ResponseRoute(null)]
            public void OutcomingNull() { }
            
            [Route("BasicRoute")]
            [ResponseRoute("")]
            public void OutcomingEmpty() { }
        }
        
        public class ExampleSubscriberWithOutcomingRouteButWithoutIcoming
        {
            public void NormalMethod() { }
            
            [ResponseRoute("SomeRoute")]
            public void Outcomingnull() { }
        }
        
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
        
        public class ExampleSubscriberWithVariousResponseConfiguration
        {
            public void NormalMethod() { }
            
            [Route("RouteWithoutResponse")]
            public void RouteWithoutResponse() { }

            [Route("RouteWithResponse")]
            [ResponseRoute("TargetRoute")]
            public void RouteWithResponse(byte[] data) { }
        }
    
        public class ExampleSubscriberWithCallHandler
        {
            public string PassedValue { get; set; }

            [Route("RouteA")]
            public void MethodA(string value) => PassedValue = value;

            [Route("RouteB")]
            public void MethodB(string value) => PassedValue = value;
        }

        #endregion
        
        [TestCase(typeof(ExampleSubscriberWithNullIncomingRoute))]
        [TestCase(typeof(ExampleSubscriberWithNullOutcomingRoute))]
        public void HandleRoutesWithNameSetToNull(Type subscriberType)
        {
            Assert.Throws<NetmqRouterException>(() =>
            {
                var subscriber = Activator.CreateInstance(subscriberType);
                ClassAnalyzer.AnalyzeClass(subscriber);
            });
        }

        [Test]
        public void HandleMethodWithRouteResponseAttributeButwithoutBasicRoute()
        {
            var subscriber = new ExampleSubscriberWithOutcomingRouteButWithoutIcoming();
            
            Assert.Throws<NetmqRouterException>(() => { ClassAnalyzer.AnalyzeClass(subscriber); });
        }
        
        [TestCase("NormalMethod", ExpectedResult = false)]
        [TestCase("GetEvent", ExpectedResult = true)]
        [TestCase("GetRaw", ExpectedResult = true)]
        [TestCase("GetText", ExpectedResult = true)]
        [TestCase("GetObject", ExpectedResult = true)]
        public bool DiscoverOnlyMethodsWithRouteAttribute(string routeName)
        {
            var _object = new ExampleSubscriberWithIncommingRoutes();
            
            return ClassAnalyzer
                .AnalyzeClass(_object)
                .Any(x => x.Incoming.Name == routeName);
        }
        
        [TestCase("GetEvent", ExpectedResult = typeof(void))]
        [TestCase("GetRaw", ExpectedResult = typeof(byte[]))]
        [TestCase("GetText", ExpectedResult = typeof(string))]
        [TestCase("GetObject", ExpectedResult = typeof(ExampleObject))]
        public Type CorrectlyDiscoverIncomingDataType(string methodName)
        {
            var _object = new ExampleSubscriberWithIncommingRoutes();
            
            return ClassAnalyzer
                .AnalyzeClass(_object)
                .First(x => x.Incoming.Name == methodName)
                .Incoming
                .DataType;
        }
        
        [TestCase("RouteWithoutResponse", ExpectedResult = null)]
        [TestCase("RouteWithResponse", ExpectedResult = "TargetRoute")]
        public string CorrectlyHandleRoutesWithoutRouteResponseAttribute(string incomingROuteName)
        {
            var _object = new ExampleSubscriberWithVariousResponseConfiguration();
            
            return ClassAnalyzer
                .AnalyzeClass(_object)
                .First(x => x.Incoming.Name == incomingROuteName)
                .Outcoming
                ?.Name;
        }

        [TestCase("NormalMethod", ExpectedResult = false)]
        [TestCase("ResponseEvent", ExpectedResult = true)]
        [TestCase("ResponseRaw", ExpectedResult = true)]
        [TestCase("ResponseText", ExpectedResult = true)]
        [TestCase("ResponseObject", ExpectedResult = true)]
        public bool DiscoverOnlyMethodsWithRouteResponseAttribute(string methodName)
        {
            var _object = new ExampleSubscriberWithOutcomingRoutes();
            
            return ClassAnalyzer
                .AnalyzeClass(_object)
                .Any(x => x.Outcoming.Name == methodName);
        }

        [TestCase("ResponseEvent", ExpectedResult = typeof(void))]
        [TestCase("ResponseRaw", ExpectedResult = typeof(byte[]))]
        [TestCase("ResponseText", ExpectedResult = typeof(string))]
        [TestCase("ResponseObject", ExpectedResult = typeof(ExampleObject))]
        public Type CorrectlyDiscoverOutcomingDataType(string methodName)
        {
            var _object = new ExampleSubscriberWithOutcomingRoutes();
            return ClassAnalyzer
                .AnalyzeClass(_object)
                .First(x => x.Outcoming.Name == methodName)
                .Outcoming
                .DataType;
        }

        [TestCase("RouteA", "MessageA")]
        [TestCase("RouteB", "MessageB")]
        public void CorrectlyCallRoute(string routeName, string value)
        {
            // arrange
            var _object = new ExampleSubscriberWithCallHandler();
            var routes = ClassAnalyzer.AnalyzeClass(_object);

            // act
            routes
                .First(x => x.Incoming.Name == routeName)
                .Method(value);

            // assert
            Assert.AreEqual(value, _object.PassedValue);
        }
    }
}
