using Azure.Messaging.ServiceBus;
using Moq;
using NUnit.Framework;
using Ridvay.Azure.ServiceBus.Client.UnitTest.Stubs;

namespace Ridvay.Azure.ServiceBus.Client.UnitTest
{
    public class MessageSenderTests
    {
        private Mock<IConsumerAttributeParserService> _attributeParser;
        private Mock<IServiceBusClientManager> _clientManager;
        private Mock<IMessageSerialize> _messageSerialize;
        private Mock<IServiceBusSenderWrapped> _sender;

        [SetUp]
        public void Setup()
        {
            _clientManager = new Mock<IServiceBusClientManager>();
            _sender = new Mock<IServiceBusSenderWrapped>();
            _attributeParser = new Mock<IConsumerAttributeParserService>();
            _messageSerialize = new Mock<IMessageSerialize>();
        }

        [Test]
        public void Should_Call_Send_Message_Once()
        {
            _clientManager
                .Setup(a => a.CreateSender(It.IsAny<string>()))
                .Returns(_sender.Object);

            _messageSerialize
                .Setup(a => a.Serialize(It.IsAny<object>()))
                .Returns("");

            var msg = new MessageSender(_clientManager.Object, _attributeParser.Object, _messageSerialize.Object);

            msg.SendAsync(new MessageDummyType { Value1 = "OK" }).Wait();

            _clientManager.Verify(a => a.CreateSender(It.IsAny<string>()), Times.Once);
            _sender.Verify(a => a.SendMessageAsync<object>(It.IsAny<ServiceBusMessage>()), Times.Once);
        }
    }
}