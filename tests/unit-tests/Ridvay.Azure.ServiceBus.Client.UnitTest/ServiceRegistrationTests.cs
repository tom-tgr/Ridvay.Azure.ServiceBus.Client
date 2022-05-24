using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Ridvay.Azure.ServiceBus.Client.Abstractions;
using Ridvay.Azure.ServiceBus.Client.UnitTest.Stubs;

namespace Ridvay.Azure.ServiceBus.Client.UnitTest
{
    public class ServiceRegistrationTests
    {
        private const string ConnectionStringDummyValue = "Endpoint=123;SharedAccessKeyName=123;SharedAccessKey=123";
        [Test]
        public void Should_Register_Default_Implementations()
        {
            var serviceProvider = new ServiceCollection()
                .AddServiceBus(ConnectionStringDummyValue)
                .BuildServiceProvider();
            
            serviceProvider.GetRequiredService<IMessageSender>();
            serviceProvider.GetRequiredService<IServiceBusAdministrator>();
            serviceProvider.GetRequiredService<IServiceBusClientManager>();
            serviceProvider.GetRequiredService<IConsumerAttributeParserService>();
            serviceProvider.GetRequiredService<IMessageSerialize>();

        }

        [Test]
        public void Should_Register_Custom_Message_Serializer()
        {
            var serviceProvider = new ServiceCollection()
                .AddServiceBus(ConnectionStringDummyValue)
                .AddTransient<IMessageSerialize, CustomMessageSerialize>()
                .BuildServiceProvider();

            
            var serializer =  serviceProvider.GetRequiredService<IMessageSerialize>();
            Assert.AreEqual(typeof(CustomMessageSerialize), serializer.GetType());
            Assert.AreEqual("👍", serializer.Serialize(new CustomMessageSerialize()));
        }

        [Test]
        public void Should_Register_Message_Sender()
        {
            var serviceProvider = new ServiceCollection()
                .AddServiceBus(ConnectionStringDummyValue)
                .BuildServiceProvider();


            var serializer = serviceProvider.GetRequiredService<IMessageSender>();

            Assert.AreEqual(typeof(MessageSender), serializer.GetType());
        }

        [Test]
        public void Should_Register_RequestReplay_Consumer()
        {
            var serviceProvider = new ServiceCollection()
                .AddServiceBus(ConnectionStringDummyValue)
                .AddConsumer<MessageConsumerRequestReplayStub>()
                .BuildServiceProvider();


            var service = serviceProvider.GetRequiredService<IMessageConsumer<MessageDummyType, MessageDummyType>>();

            var processors = serviceProvider.GetServices<IHostedService>();
            Assert.AreEqual(typeof(MessageConsumerRequestReplayStub), service.GetType());
            Assert.IsTrue(processors.Any(a=>a.GetType() == typeof(MessageConsumerRequestReplayMessageService<MessageDummyType, MessageDummyType>)));
        }
        [Test]
        public void Should_Throw_On_Same_RequestReplay_Consumer()
        {
            Assert.Throws<ArgumentException>(() => new ServiceCollection()
                .AddServiceBus(ConnectionStringDummyValue)
                .AddConsumer<MessageConsumerRequestReplayStub>()
                .AddConsumer<MessageConsumerRequestReplayStub>()
                .BuildServiceProvider());

        }

        [Test]
        public void Should_Throw_On_Same_RequestReplay_Consumer_Implementation()
        {
            Assert.Throws<ArgumentException>(() => new ServiceCollection()
                .AddServiceBus(ConnectionStringDummyValue)
                .AddConsumer<MessageConsumerRequestReplayStub>()
                .AddConsumer<MessageConsumerRequestReplay_SameImplementationStub>()
                .BuildServiceProvider());
            
        }


        [Test]
        public void Should_Register_One_Class_With_Multiple_RequestReplay_Consumer()
        {
            var serviceProvider = new ServiceCollection()
                .AddServiceBus(ConnectionStringDummyValue)
                .AddConsumer<MessageConsumerRequestReplayMultipleConsumersStub>()
                .BuildServiceProvider();

            
            var service1 = serviceProvider.GetRequiredService<IMessageConsumer<MessageDummyType, MessageDummyType>>();
            var service2 = serviceProvider.GetRequiredService<IMessageConsumer<MessageDummyType2, MessageDummyType2>>();
            

            var processors = serviceProvider.GetServices<IHostedService>().ToList();
            Assert.AreEqual(typeof(MessageConsumerRequestReplayMultipleConsumersStub), service1.GetType());
            Assert.AreEqual(typeof(MessageConsumerRequestReplayMultipleConsumersStub), service2.GetType());
            Assert.IsTrue(processors.Any(a => a.GetType() == typeof(MessageConsumerRequestReplayMessageService<MessageDummyType, MessageDummyType>)));
            Assert.IsTrue(processors.Any(a => a.GetType() == typeof(MessageConsumerRequestReplayMessageService<MessageDummyType2, MessageDummyType2>)));
        }
        [Test]
        public void Should_Register_Void_Consumer()
        {
            var serviceProvider = new ServiceCollection()
                .AddServiceBus(ConnectionStringDummyValue)
                .AddConsumer<MessageConsumerVoidStub>()
                .BuildServiceProvider();


            var service = serviceProvider.GetRequiredService<IMessageConsumer<MessageDummyType>>();

            var processors = serviceProvider.GetServices<IHostedService>();
            Assert.AreEqual(typeof(MessageConsumerVoidStub), service.GetType());
            Assert.IsTrue(processors.Any(a => a.GetType() == typeof(MessageConsumerVoidMessageService<MessageDummyType>)));
        }

        [Test]
        public void Should_Throw_On_Same_Void_Consumer()
        {
            Assert.Throws<ArgumentException>(() =>
                new ServiceCollection()
                    .AddServiceBus(ConnectionStringDummyValue)
                    .AddConsumer<MessageConsumerVoidStub>()
                    .AddConsumer<MessageConsumerVoidStub>()
                    .BuildServiceProvider());
        }
        
        [Test]
        public void Should_Throw_On_Same_Void_Consumer_Implementation()
        {
            Assert.Throws<ArgumentException>(() =>
                new ServiceCollection()
                    .AddServiceBus(ConnectionStringDummyValue)
                    .AddConsumer<MessageConsumerVoidStub>()
                    .AddConsumer<MessageConsumerVoidStub_SameImplementationStub>()
                    .BuildServiceProvider());
        }

        [Test]
        public void Should_Throw_On_Same_Message_Type_On_RequestReplay_And_Void()
        {
            Assert.Throws<ArgumentException>(() =>
                new ServiceCollection()
                    .AddServiceBus(ConnectionStringDummyValue)
                    .AddConsumer<MessageConsumerRequestReplayStub>()
                    .AddConsumer<MessageConsumerVoidStub>()
                    .BuildServiceProvider());
        }

        [Test]
        public void Should_Throw_On_Same_Message_Type_On_Void_And_RequestReplay()
        {
            Assert.Throws<ArgumentException>(() =>
                new ServiceCollection()
                    .AddServiceBus(ConnectionStringDummyValue)
                    .AddConsumer<MessageConsumerVoidStub>()
                    .AddConsumer<MessageConsumerRequestReplayStub>()
                    .BuildServiceProvider());
        }

        [Test]
        public void Should_Throw_On_Bad_Consumer_Implementation()
        {
            Assert.Throws<ArgumentException>(() =>
                new ServiceCollection()
                    .AddServiceBus(ConnectionStringDummyValue)
                    .AddConsumer<MessageConsumerWithIMessageConsumerStub>()
                    .BuildServiceProvider());
        }
        
    }
}