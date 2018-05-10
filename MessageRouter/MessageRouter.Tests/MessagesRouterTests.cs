using System;
using System.Threading.Tasks;
using MessageRouter.Attributes;
using MessageRouter.Fluent;
using MessageRouter.Json;
using MessageRouter.NetMQ;
using MessageRouter.Serialization;
using NetMQ.Sockets;
using NUnit.Framework;

namespace NetmqRouter.Tests
{
    [TestFixture]
    public class MessagesRouterTests
    {
        private const string Address = "tcp://localhost:6000";

        // will be serialized as JSON
        class CustomPayload
        {
            public string Text { get; set; }
            public int Number { get; set; }

            public CustomPayload(string text, int number)
            {
                Text = text;
                Number = number;
            }

            public override bool Equals(object obj)
            {
                return obj is CustomPayload o &&
                       this.Number == o.Number;
            }
        }

        class ExampleSubscriber
        {
            public CustomPayload PassedValue;

            [Route("TestRoute")]
            public void Test(CustomPayload value)
            {
                PassedValue = value;
            }
        }

        [Test]
        public async Task RoutingTest()
        {
            var publisherSocket = new PublisherSocket();
            publisherSocket.Bind(Address);

            var subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(Address);

            var subscriber = new ExampleSubscriber();

            var router = NetmqMessageRouter
                .WithPubSubConnecton(publisherSocket, subscriberSocket)
                .RegisterTypeSerializer(new RawDataTypeSerializer())
                .RegisterTypeSerializer(new BasicTextTypeSerializer())
                .RegisterGeneralSerializer(new JsonObjectSerializer())
                .RegisterRoute("TestRoute", typeof(CustomPayload))
                .RegisterSubscriber(subscriber)
                .StartRouting();

            router.SendMessage("TestRoute", new CustomPayload("Hellow world", 123));

            router.OnException += exception =>
            {
                // handle any exception
            };

            await Task.Delay(TimeSpan.FromSeconds(3));

            router
                .StopRouting()
                .Disconnect();

            var expectedValue = new CustomPayload("Hellow world", 123);
            Assert.AreEqual(expectedValue, subscriber.PassedValue);
        }
    }
}
