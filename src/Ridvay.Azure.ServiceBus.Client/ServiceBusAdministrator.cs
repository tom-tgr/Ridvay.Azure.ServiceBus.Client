using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Options;

namespace Ridvay.Azure.ServiceBus.Client
{
    public interface IServiceBusAdministrator
    {
        Task CreateTopicIfNotExistsAsync(string topic);
        Task CreateQueueIfNotExistAsync(string queueName);
        Task CreateQueueIfNotExistAsync(CreateQueueOptions options);
        Task CreateTopicOrQueueIfNotExistsAsync<T>();
        Task TryRemoveTopicOrQueueAsync<T>();
    }

    internal class ServiceBusAdministrator : IServiceBusAdministrator
    {
        private static ConcurrentDictionary<string, DateTime> _queueExistsStore;
        private readonly IConsumerAttributeParserService _attributeParserService;
        private readonly ServiceBusAdministrationClient _client;

        public ServiceBusAdministrator(
            IOptions<ServiceBusSettings> settings,
            IConsumerAttributeParserService attributeParserService)
        {
            _attributeParserService = attributeParserService;
            _client = new ServiceBusAdministrationClient(settings.Value.ConnectionString);

            _queueExistsStore = new ConcurrentDictionary<string, DateTime>();
        }


        public async Task CreateTopicOrQueueIfNotExistsAsync<T>()
        {
            var name = _attributeParserService.GetTopicOrQueueName<T>();
            if (_attributeParserService.IsTopicProcessor<T>())
                await CreateTopicIfNotExistsAsync(name);

            else
                await CreateQueueIfNotExistAsync(name);
        }

        public async Task CreateQueueIfNotExistAsync(CreateQueueOptions options)
        {
            if (!await MemoizedQueueExists(options.Name))
                await _client.CreateQueueAsync(options);
        }

        public async Task CreateQueueIfNotExistAsync(string topic)
        {
            await CreateQueueIfNotExistAsync(new CreateQueueOptions(topic));
        }

        public async Task CreateTopicIfNotExistsAsync(string queueName)
        {
            if (!await _client.TopicExistsAsync(queueName))
                await _client.CreateTopicAsync(queueName);
        }

        public async Task TryRemoveTopicOrQueueAsync<T>()
        {
            try
            {
                var name = _attributeParserService.GetTopicOrQueueName<T>();

                if (_attributeParserService.IsTopicProcessor<T>())
                    await _client.DeleteTopicAsync(name);
                else
                    await _client.DeleteQueueAsync(name);
            }
            catch
            {
                // ignored
            }
        }

        private async Task<bool> MemoizedQueueExists(string name)
        {
            if (_queueExistsStore.ContainsKey(name)) return true;

            if (!await _client.QueueExistsAsync(name)) return false;


            _queueExistsStore.TryAdd(name, DateTime.Now);
            return true;
        }
    }
}