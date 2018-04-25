using Moq;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetmqRouter.Workers;
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
            SerializedMessage message = default;
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
    }
}