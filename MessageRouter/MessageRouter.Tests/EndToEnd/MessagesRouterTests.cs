using System;
using System.Threading.Tasks;
using MessageRouter.Attributes;
using MessageRouter.Fluent;
using MessageRouter.Json;
using MessageRouter.NetMQ;
using MessageRouter.Serialization;
using NetMQ.Sockets;
using NUnit.Framework;

namespace NetmqRouter.Tests.EndToEnd
{
    [TestFixture]
    public class MessagesRouterTests
    {
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

        class Vector
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        [Test]
        public async Task RoutingTestForClassSubscriber()
        {
            var address = "tcp://localhost:6000";
            
            var publisherSocket = new PublisherSocket();
            publisherSocket.Bind(address);

            var subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(address);

            var subscriber = new ExampleSubscriber();

            var router = NetmqMessageRouter
                .WithPubSubConnecton(publisherSocket, subscriberSocket)
                .WithWorkerPool(numberOfSerializationWorkes: 2, numberOfHandlingWorkes: 8)
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

            await Task.Delay(TimeSpan.FromSeconds(5));

            router
                .StopRouting()
                .Disconnect();

            var expectedValue = new CustomPayload("Hellow world", 123);
            Assert.AreEqual(expectedValue, subscriber.PassedValue);
        }
        
        [Test]
        public async Task RoutingTestForCustomSubscriber()
        {
            var address = "tcp://localhost:6001";
            
            var publisherSocket = new PublisherSocket();
            publisherSocket.Bind(address);

            var subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(address);

            var router = NetmqMessageRouter
                .WithPubSubConnecton(publisherSocket, subscriberSocket)
                .RegisterJsonSerializer()
                .RegisterRoute("VectorRoute", typeof(Vector))
                .RegisterRoute("VectorLengthRoute", typeof(double));
            
            router
                .Subscribe("VectorRoute")
                .WithResponse("VectorLengthRoute")
                .WithHandler((Vector vector) =>
                {
                    return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
                });

            double? lengthAnswer = null;
            
            router
                .Subscribe("VectorLengthRoute")
                .WithHandler((double x) => lengthAnswer = x);

            router.StartRouting();

            router
                .SendMessage(new Vector() { X = 3, Y = 4 })
                .To("VectorRoute");
            
            await Task.Delay(TimeSpan.FromSeconds(5));

            router
                .StopRouting()
                .Disconnect();

            Assert.AreEqual(5d, lengthAnswer);
        }
    }
}
