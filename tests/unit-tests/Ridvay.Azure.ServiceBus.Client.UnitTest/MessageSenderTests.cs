using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Moq;
using Ridvay.Azure.ServiceBus.Client.UnitTest.Stubs;

namespace Ridvay.Azure.ServiceBus.Client.UnitTest
{
    public class MessageSenderTests
    {
        private Mock<IServiceBusClientManager> clientManager;
        private Mock<IServiceBusSenderWrapped> sender;
        private Mock<IConsumerAttributeParserService> attributeParser;
        private Mock<IServiceBusAdministrator> busAdministrator;
        private Mock<IMessageSerialize> messageSerialize;

        [SetUp]
        public void Setup()
        {
            clientManager = new Mock<IServiceBusClientManager>();
            sender = new Mock<IServiceBusSenderWrapped>();

            attributeParser = new Mock<IConsumerAttributeParserService>();
            busAdministrator = new Mock<IServiceBusAdministrator>();
            messageSerialize = new Mock<IMessageSerialize>();
        }

        [Test]
        public void Should_Call_Send_Message_Once()
        {
            clientManager
                .Setup(a => a.CreateSender(It.IsAny<string>()))
                .Returns(sender.Object);

            messageSerialize
                .Setup(a => a.Serialize(It.IsAny<object>()))
                .Returns("");
        
            var msg =  new MessageSender(clientManager.Object, attributeParser.Object, busAdministrator.Object,
                messageSerialize.Object);
           
            msg.SendAsync(new MessageDummyType() { Value1 = "OK" }).Wait();

            clientManager.Verify(a=>a.CreateSender(It.IsAny<string>()), Times.Once);
            sender.Verify(a=>a.SendMessageAsync<object>(It.IsAny<ServiceBusMessage>()), Times.Once);
        }
    }
}
