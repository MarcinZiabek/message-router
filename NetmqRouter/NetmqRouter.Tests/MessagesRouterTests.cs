using System;
using System.Threading.Tasks;
using NetmqRouter.Attributes;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Serialization;
using NetMQ.Sockets;
using NUnit.Framework;

namespace NetmqRouter.Tests
{
    [TestFixture]
    public class MessagesRouterTests
    {
        private const string Address = "tcp://localhost:50003";

        class ExampleSubscriber
        {
            public string PassedValue = "";

            [Route("TestRoute")]
            public void Test(string value)
            {
                PassedValue = value;
            }
        }

        [Test]
        public async Task IncomingRouteNameWithoutBaseRoute()
        {
            var publisherSocket = new PublisherSocket();
            publisherSocket.Bind(Address);

            var subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(Address);

            var subscriber = new ExampleSubscriber();

            var router = MessageRouter
                .WithPubSubConnecton(publisherSocket, subscriberSocket)
                .RegisterTypeSerializerFor(new BasicTextTypeSerializer())
                .RegisterRoute("TestRoute", typeof(string))
                .Subscribe(subscriber)
                .StartRouting();

            router.SendMessage("TestRoute", "test");

            await Task.Delay(TimeSpan.FromSeconds(3));

            router
                .StopRouting()
                .Disconnect();

            Assert.AreEqual("test", subscriber.PassedValue);
        }
    }
}
