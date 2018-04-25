using Moq;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;
using NetmqRouter.Workers;
using NUnit.Framework;

namespace NetmqRouter.Tests.Workers
{
    [TestFixture]
    public class MessageSerializerTests
    {
        [Test]
        public void WorkerLoopWhenNoMessages()
        {
            // arrange
            var eventWasInvoked = false;
            
            var dataContract = new Mock<IDataContract>();
            dataContract.Setup(x => x.Serialize(It.IsAny<Message>())).Returns(null);
            
            var worker = new MessageSerializer(dataContract.Object);
            worker.OnNewMessage += _ => eventWasInvoked = true;
            
            // act
            var messageProcessed = worker.DoWork();
            
            // assert
            Assert.IsFalse(messageProcessed);
            Assert.IsFalse(eventWasInvoked);
            
            dataContract.Verify(x => x.Serialize(It.IsAny<Message>()), Times.Never);
        }
        
        [Test]
        public void WorkerLoopWhenSingleMessage()
        {
            // arrange
            var message = new Message("IncomingRoute", "test");
            var serializedMessage = new SerializedMessage("OutcomingRoute", new byte[4]);
            SerializedMessage? messageFromEvent = null;
            
            var dataContract = new Mock<IDataContract>();
            dataContract.Setup(x => x.Serialize(message)).Returns(serializedMessage);
            
            var worker = new MessageSerializer(dataContract.Object);
            worker.OnNewMessage += m => messageFromEvent = m;
            
            // act
            worker.SerializeMessage(message);
            var messageProcessed = worker.DoWork();
            
            // assert
            Assert.IsTrue(messageProcessed);
            Assert.AreEqual(serializedMessage, messageFromEvent);
            
            dataContract.Verify(x => x.Serialize(message), Times.Once);
        }
    }
}