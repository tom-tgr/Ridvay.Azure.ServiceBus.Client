using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Ridvay.Azure.ServiceBus.Client.Abstractions;

namespace Ridvay.Azure.ServiceBus.Client
{
    internal class MessageConsumerVoidMessageService<T> : MessageConsumerServiceBase where T : class
    {
        private readonly IConsumerAttributeParserService _attributeParser;
        private readonly IMessageConsumer<T> _consumer;
        private readonly IMessageSerialize _messageSerialize;

        public MessageConsumerVoidMessageService(IMessageConsumer<T> consumer, IServiceBusClientManager clientFactory,
            IServiceBusAdministrator busAdministrator,
            IConsumerAttributeParserService attributeParser,
            IMessageSerialize messageSerialize) : base(clientFactory, busAdministrator)
        {
            _consumer = consumer;
            _attributeParser = attributeParser;
            _messageSerialize = messageSerialize;
        }

        protected override bool IsTopic => _attributeParser.IsTopicProcessor<T>();
        protected override string TopicOrQueueName => _attributeParser.GetTopicOrQueueName<T>();
        protected override ServiceBusProcessorOptions Options => _attributeParser.GetOptions<T>();

        protected override TopicConsumerAttribute TopicConsumerAttribute =>
            _attributeParser.GetTopicConsumerAttribute<T>();

        protected override async Task ProcessMessage(ProcessMessageEventArgs args)
        {
            var value = _messageSerialize.Deserialize<T>(args.Message.Body.ToString());
            var retValue = new MessageContext<T>(value, args);
            await _consumer.ConsumeAsync(retValue);
        }
    }
}