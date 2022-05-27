using System.Threading.Tasks;
using Ridvay.Azure.ServiceBus.Client.Abstractions;

namespace Ridvay.Azure.ServiceBus.Client.UnitTest.Stubs
{
    public class MessageConsumerRequestReplayStub : IMessageConsumer<MessageDummyType, MessageDummyType>
    {
        public Task<MessageDummyType> ConsumeAsync(IMessageContext<MessageDummyType> message)
        {

            return Task.FromResult(new MessageDummyType() { Value1 = "OK" });
        }
    }
    // ReSharper disable once InconsistentNaming
    public class MessageConsumerRequestReplay_SameImplementationStub : IMessageConsumer<MessageDummyType, MessageDummyType>
    {
        public Task<MessageDummyType> ConsumeAsync(IMessageContext<MessageDummyType> message)
        {

            return Task.FromResult(new MessageDummyType() { Value1 = "OK" });
        }
    }
    public class MessageConsumerRequestReplayMultipleConsumersStub : 
        IMessageConsumer<MessageDummyType, MessageDummyType>,
        IMessageConsumer<MessageDummyType2, MessageDummyType2>
    {
        public Task<MessageDummyType> ConsumeAsync(IMessageContext<MessageDummyType> message)
        {

            return Task.FromResult(new MessageDummyType() { Value1 = "OK" });
        }

        public Task<MessageDummyType2> ConsumeAsync(IMessageContext<MessageDummyType2> message)
        {
            return Task.FromResult(new MessageDummyType2() { Value1 = "OK" });
        }
    }
    public class MessageConsumerVoidStub : IMessageConsumer<MessageDummyType>
    {
        public Task ConsumeAsync(IMessageContext<MessageDummyType> message)
        {
            var a = message;

            return Task.CompletedTask;
        }
    }
    public class MessageConsumerVoidStub_SameImplementationStub : IMessageConsumer<MessageDummyType>
    {
        public Task ConsumeAsync(IMessageContext<MessageDummyType> message)
        {
            var a = message;

            return Task.CompletedTask;
        }
    }
    public class MessageConsumerWithIMessageConsumerStub : IMessageConsumer
    {
    }
}