using System;
using Moq;
using NetmqRouter.Exceptions;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetmqRouter.Workers;
using NUnit.Framework;

namespace NetmqRouter.Tests.Workers
{
    [TestFixture]
    public class MessageHandlerTests
    {
        [Test]
        public void WorkerLoopWhenNoMessages()
        {
            // arrange
            var eventWasInvoked = false;

            var dataContract = new Mock<IDataContractOperations>();
            dataContract.Setup(x => x.CallRoute(It.IsAny<Message>())).Returns(new Message[0]);

            var worker = new MessageHandler(dataContract.Object);
            worker.OnNewMessage += _ => eventWasInvoked = true;

            // act
            var messageProcessed = worker.DoWork();

            // assert
            Assert.IsFalse(messageProcessed);
            Assert.IsFalse(eventWasInvoked);

            dataContract.Verify(x => x.CallRoute(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public void WorkerLoopWhenSingleMessage()
        {
            // arrange
            var incomingMessage = new Message("IncomingRoute", "IncomingPayload");
            var outcomingMessage = new Message("OutcomingRoute", "OutcomingPayload");
            Message messageFromEvent = null;

            var dataContract = new Mock<IDataContractOperations>();
            dataContract.Setup(x => x.CallRoute(incomingMessage)).Returns(new[] {outcomingMessage});

            var worker = new MessageHandler(dataContract.Object);
            worker.OnNewMessage += m => messageFromEvent = m;

            // act
            worker.HandleMessage(incomingMessage);
            var messageProcessed = worker.DoWork();

            // assert
            Assert.IsTrue(messageProcessed);
            Assert.AreEqual(outcomingMessage, messageFromEvent);

            dataContract.Verify(x => x.CallRoute(incomingMessage), Times.Once);
        }

        [Test]
        public void OnException()
        {
            // arrange
            var dataContract = new Mock<IDataContractOperations>();
            dataContract.Setup(x => x.CallRoute(It.IsAny<Message>())).Throws<Exception>();

            var worker = new MessageHandler(dataContract.Object);
            var message = new Message("IncomingRoute", "IncomingPayload");

            // act
            worker.HandleMessage(message);

            // assert
            Assert.Throws<SubscriberException>(() =>
            {
                worker.DoWork();
            });
        }
    }
}