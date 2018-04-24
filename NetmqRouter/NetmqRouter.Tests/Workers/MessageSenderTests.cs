using Moq;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetmqRouter.Workers;
using NUnit.Framework;

namespace NetmqRouter.Tests.Workers
{
    [TestFixture]
    public class MessageSenderTests
    {
        [Test]
        public void WorkerLoopWhenQueueIsEmpty()
        {
            // arrange
            var connection = new Mock<IConnection>();
            var worker = new MessageSender(connection.Object);
            
            // act
            var messageProcessed = worker.DoWork();
            
            // assert
            Assert.IsFalse(messageProcessed);
            
            connection.Verify(x => x.SendMessage(It.IsAny<SerializedMessage>()), Times.Never);
        }
        
        [Test]
        public void WorkerLoopWhenQueueContainsMessage()
        {
            // arrange
            var connection = new Mock<IConnection>();
            var worker = new MessageSender(connection.Object);
            var message = new SerializedMessage("TestRoute", new byte[] { 1, 2, 3 });
            
            // act
            worker.SendMessage(message);
            var firstProcessing = worker.DoWork();
            var secondProcessing = worker.DoWork();
            
            // assert
            Assert.IsTrue(firstProcessing);
            Assert.IsFalse(secondProcessing);
            
            connection.Verify(x => x.SendMessage(message), Times.Once);
        }
    }
}