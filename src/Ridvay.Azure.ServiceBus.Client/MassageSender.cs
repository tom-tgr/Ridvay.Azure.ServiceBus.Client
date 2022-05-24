using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace Ridvay.Azure.ServiceBus.Client
{
    public class MessageSender : IMessageSender
    {
        private readonly IServiceBusClientManager _clientManager;
        private readonly IConsumerAttributeParserService _attributeParser;
        private readonly IServiceBusAdministrator _busAdministrator;
        private readonly IMessageSerialize _messageSerialize;

        private static BlockingCollection<string> _created;

        public MessageSender(IServiceBusClientManager clientManager,
        IConsumerAttributeParserService attributeParser, 
        IServiceBusAdministrator busAdministrator,
        IMessageSerialize messageSerialize)
        {
            _clientManager = clientManager;
            _attributeParser = attributeParser;
            _busAdministrator = busAdministrator;
            _messageSerialize = messageSerialize;

            _created ??= new BlockingCollection<string>();
        }
        
        public async Task SendAsync<T>(T msg)
        {

            var message = new ServiceBusMessage(_messageSerialize.Serialize<T>(msg));
            await SendInternal<T>(message);
        }

        public async Task ScheduledSendAsync<T>(T msg, DateTime scheduledEnqueueTimeUtc)
        {
            var message = new ServiceBusMessage(_messageSerialize.Serialize<T>(msg))
            {
                ScheduledEnqueueTime = scheduledEnqueueTimeUtc
            };
            await SendInternal<T>(message);
        }

        private async Task SendInternal<T>(ServiceBusMessage message)
        {
            var name = _attributeParser.GetTopicOrQueueName<T>();

            var sender = _clientManager.CreateSender(name);

            await sender.SendMessageAsync<T>(message);
        }

        public async Task<TResponse> GetAsync<T, TResponse>(T msg)

        {
            var name = _attributeParser.GetTopicOrQueueName<T>();
            var msgId = "replay_" + Guid.NewGuid();
            var client = _clientManager.CreateBusClient();
            
            var sender = client.CreateSender(name);
            var message = new ServiceBusMessage(_messageSerialize.Serialize(msg))
            {
                SessionId = msgId,
                ReplyToSessionId = msgId
            };
           
            await sender.SendMessageAsync<T>(message);

            var replayQName = _attributeParser.GetReplayQueueName<T>();
            var receiver = await client.ReplayToQueue(replayQName, msgId);
            
            var receivedMessage = await receiver.ReceiveMessageAsync(TimeSpan.FromMinutes(5));

            var result = receivedMessage.Body.ToString();
            

            return _messageSerialize.Deserialize<TResponse>(result);
        }
        
    }
} 