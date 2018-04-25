using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetmqRouter;
using NetmqRouter.BusinessLogic;
using NetmqRouter.Tests;
using NetMQ.Sockets;

namespace Playground
{
    class Program
    {
        private const string Address = "tcp://localhost:50000";

        static void Main(string[] args)
        {
            var publisherSocket = new PublisherSocket();
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

            Thread.Sleep(TimeSpan.FromSeconds(3));
            Console.WriteLine(subscriber.CalledMethod);

            router
                .StopRouting()
                .Disconnect();

            Console.Read();
        }
    }
}
