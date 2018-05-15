using System;
using System.Threading.Tasks;
using MessageRouter.Json;
using MessageRouter.Fluent;
using MessageRouter.NetMQ;
using NetMQ.Sockets;
using NUnit.Framework;

namespace NetmqRouter.Tests
{
    [TestFixture]
    public class Fluent
    {
        [Test]
        public async Task BasicEvent()
        {
            var address = "tcp://localhost:6002";
            
            var publisherSocket = new PublisherSocket();
            publisherSocket.Bind(address);

            var subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(address);

            var router = NetmqMessageRouter
                .WithPubSubConnecton(publisherSocket, subscriberSocket)
                .RegisterJsonSerializer()
                .RegisterEventRoute("EventSource")
                .RegisterRoute("EventReply", typeof(double));
            
            router
                .Subscribe("EventSource")
                .WithResponse("EventReply")
                .WithHandler(() => 15d);

            double? answer = null;
            
            router
                .Subscribe("EventReply")
                .WithHandler((double x) => 
                    answer = x);

            router.OnException += exp =>
            {
                var i = 15;
            };

            router.StartRouting();

            router
                .SendEvent()
                .To("EventSource");
            
            await Task.Delay(TimeSpan.FromSeconds(5));

            router
                .StopRouting()
                .Disconnect();

            Assert.AreEqual(15d, answer);
        }
    }
}