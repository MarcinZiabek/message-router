using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ.Sockets;
using NUnit.Framework;

namespace NetmqRouter.Tests
{
    [TestFixture]
    public class MessagesRouterTests
    {
        private const string Address = "tcp://localhost:50000";

        [Test]
        public void IncomingRouteNameWithoutBaseRoute()
        {
            var publisherSocket = new PublisherSocket();
            publisherSocket.Bind(Address);

            var subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(Address);

            var router = new MessageRouter(publisherSocket, subscriberSocket)
                .Subscribe(new ExampleSubscriber());
                //.StartRouting();

            router.Dispose();
        }
    }
}
