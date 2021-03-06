﻿using System;
using MessageRouter.Exceptions;
using MessageRouter.Infrastructure;
using MessageRouter.Models;
using MessageRouter.Workers;
using Moq;
using NUnit.Framework;

namespace NetmqRouter.Tests.Workers
{
    [TestFixture]
    public class MessageReceiverTests
    {
        [Test]
        public void WorkerLoopWhenNoMessages()
        {
            // arrange
            SerializedMessage message;
            var eventWasInvoked = false;

            var connection = new Mock<IConnection>();
            connection.Setup(x => x.TryReceiveMessage(out message)).Returns(false);

            var worker = new MessageReveiver(connection.Object);
            worker.OnNewMessage += _ => eventWasInvoked = true;

            // act
            var messageProcessed = worker.DoWork();

            // assert
            Assert.IsFalse(messageProcessed);
            Assert.IsFalse(eventWasInvoked);

            connection.Verify(x => x.TryReceiveMessage(out message), Times.Once);
        }

        [Test]
        public void WorkerLoopWhenSingleMessage()
        {
            // arrange
            var message = new SerializedMessage("TestRoute", new byte[] { 1, 2, 3 });
            SerializedMessage? messageFromEvent = null;

            var connection = new Mock<IConnection>();
            connection.Setup(x => x.TryReceiveMessage(out message)).Returns(true);

            var worker = new MessageReveiver(connection.Object);
            worker.OnNewMessage += m => messageFromEvent = m;

            // act
            var messageProcessed = worker.DoWork();

            // assert
            Assert.IsTrue(messageProcessed);
            Assert.AreEqual(message, messageFromEvent);

            connection.Verify(x => x.TryReceiveMessage(out message), Times.Once);
        }

        [Test]
        public void OnException()
        {
            // arrange
            var connection = new Mock<IConnection>();
            SerializedMessage message;
            connection.Setup(x => x.TryReceiveMessage(out message)).Throws<Exception>();

            var worker = new MessageReveiver(connection.Object);

            // assert
            Assert.Throws<ConnectionException>(() =>
            {
                worker.DoWork();
            });
        }
    }
}