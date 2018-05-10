using System;
using MessageRouter.Models;
using NUnit.Framework;

namespace NetmqRouter.Tests.Models
{
    [TestFixture]
    public class SubscriberTests
    {
        #region Test Classes

        internal class InputClass
        {

        }

        internal class OutputClass
        {

        }

        #endregion

        [Test]
        public void CreateBasicSubscriber()
        {
            // arrange
            const string routeName = "IncomingRoute";
            var isActionCalled = false;

            Action<InputClass> action = x => isActionCalled = true;

            // act
            var subsriber = Subscriber.Create(routeName, action);
            subsriber.Method(new InputClass());

            // assert
            Assert.AreEqual(routeName, subsriber.Incoming.Name);
            Assert.AreEqual(typeof(InputClass), subsriber.Incoming.DataType);

            Assert.AreEqual(null, subsriber.Outcoming);
            Assert.AreEqual(true, isActionCalled);
        }

        [Test]
        public void CreateSubscriberWithResponseRoute()
        {
            // arrange
            const string incomingRouteName = "IncomingRoute";
            const string outcomingRouteName = "OutcomingRoute";
            var isActionCalled = false;

            Func<InputClass, OutputClass> action = x =>
            {
                isActionCalled = true;
                return new OutputClass();
            };

            // act
            var subsriber = Subscriber.Create(incomingRouteName, outcomingRouteName, action);
            subsriber.Method(new InputClass());

            // assert
            Assert.AreEqual(incomingRouteName, subsriber.Incoming.Name);
            Assert.AreEqual(typeof(InputClass), subsriber.Incoming.DataType);

            Assert.AreEqual(outcomingRouteName, subsriber.Outcoming.Name);
            Assert.AreEqual(typeof(OutputClass), subsriber.Outcoming.DataType);

            Assert.AreEqual(true, isActionCalled);
        }
    }
}