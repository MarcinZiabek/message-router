using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetmqRouter.BusinessLogic;
using NetMQ.Sockets;
using NUnit.Framework;

namespace NetmqRouter.Tests
{
    [TestFixture]
    public class MessagesRouterTests
    {
        private const string Address = "tcp://localhost:50000";

        [Test]
        public async Task IncomingRouteNameWithoutBaseRoute()
        {
            /*var publisherSocket = new PublisherSocket();
            publisherSocket.Bind(Address);

            var subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(Address);

            var subscriber = new ExampleSubscriber();

            var router = MessageRouter
                .WithPubSubConnecton(publisherSocket, subscriberSocket)
                .Subscribe(subscriber)
                .StartRouting()
                as MessageRouter;

            router.SendMessage("Text", "test");

            await Task.Delay(TimeSpan.FromSeconds(2));

            Assert.AreEqual(nameof(ExampleSubscriber.TextSubscriber), subscriber.PassedValue);

            router
                .StopRouting()
                .Disconnect();*/
        }
    }
}
