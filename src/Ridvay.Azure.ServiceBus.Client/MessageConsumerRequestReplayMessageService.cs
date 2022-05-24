using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Ridvay.Azure.ServiceBus.Client.Abstractions;

namespace Ridvay.Azure.ServiceBus.Client
{
    internal class MessageConsumerRequestReplayMessageService<T,TReplay> : MessageConsumerServiceBase 
        where T : class
        where TReplay : class
    {
        private readonly IMessageConsumer<T,TReplay> _consumer;
        private readonly IServiceBusAdministrator _busAdministrator;
        private readonly IConsumerAttributeParserService _attributeParser;
        private readonly IMessageSerialize _messageSerialize;


        public MessageConsumerRequestReplayMessageService(IMessageConsumer<T, TReplay> consumer, IServiceBusClientManager clientFactory,
            IServiceBusAdministrator busAdministrator,
            IConsumerAttributeParserService attributeParser,
            IMessageSerialize messageSerialize) : base(clientFactory, busAdministrator)
        {
            _consumer = consumer;
            _busAdministrator = busAdministrator;
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
            var retValue = new MessageResponse<T>(value, args);
            var replay  = await _consumer.OnMessageAsync(retValue);


            var replayQName = _attributeParser.GetReplayQueueName<T>();
            var client = ClientManager.CreateSender(replayQName);
            await client.ReplayMessage(new ServiceBusMessage(_messageSerialize.Serialize(replay))
            {
                SessionId = args.Message.ReplyToSessionId,
                ReplyToSessionId = args.Message.ReplyToSessionId
            }, replayQName);
        }
    }
}