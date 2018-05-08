using System;
using MessageRouter.Exceptions;
using MessageRouter.Infrastructure;
using MessageRouter.Models;
using MessageRouter.Workers;
using Moq;
using NUnit.Framework;

namespace NetmqRouter.Tests.Workers
{
    [TestFixture]
    public class MessageDeserializerTests
    {
        [Test]
        public void WorkerLoopWhenNoMessages()
        {
            // arrange
            var eventWasInvoked = false;

            var dataContract = new Mock<IDataContractOperations>();
            dataContract.Setup(x => x.Deserialize(It.IsAny<SerializedMessage>()));

            var worker = new MessageDeserializer(dataContract.Object);
            worker.OnNewMessage += _ => eventWasInvoked = true;

            // act
            var messageProcessed = worker.DoWork();

            // assert
            Assert.IsFalse(messageProcessed);
            Assert.IsFalse(eventWasInvoked);

            dataContract.Verify(x => x.Deserialize(It.IsAny<SerializedMessage>()), Times.Never);
        }

        [Test]
        public void WorkerLoopWhenSingleMessage()
        {
            // arrange
            var message = new Message("IncomingRoute", "test");
            var serializedMessage = new SerializedMessage("OutcomingRoute", new byte[4]);
            Message messageFromEvent = null;

            var dataContract = new Mock<IDataContractOperations>();
            dataContract.Setup(x => x.Deserialize(serializedMessage)).Returns(message);

            var worker = new MessageDeserializer(dataContract.Object);
            worker.OnNewMessage += m => messageFromEvent = m;

            // act
            worker.DeserializeMessage(serializedMessage);
            var messageProcessed = worker.DoWork();

            // assert
            Assert.IsTrue(messageProcessed);
            Assert.AreEqual(message, messageFromEvent);

            dataContract.Verify(x => x.Deserialize(serializedMessage), Times.Once);
        }

        [Test]
        public void OnException()
        {
            // arrange
            var serializedMessage = new SerializedMessage("OutcomingRoute", new byte[4]);

            var dataContract = new Mock<IDataContractOperations>();
            dataContract.Setup(x => x.Deserialize(serializedMessage)).Throws<Exception>();

            var worker = new MessageDeserializer(dataContract.Object);

            // act
            worker.DeserializeMessage(serializedMessage);

            // assert
            Assert.Throws<SerializationException>(() =>
            {
                worker.DoWork();
            });
        }
    }
}