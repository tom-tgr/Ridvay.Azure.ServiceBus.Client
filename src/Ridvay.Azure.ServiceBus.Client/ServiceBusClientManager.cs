using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Options;
using Ridvay.Azure.ServiceBus.Client.Helpers;

namespace Ridvay.Azure.ServiceBus.Client
{
    internal class ServiceBusClientManager : IServiceBusClientManager, IAsyncDisposable
    {
        private readonly ServiceBusSettings _settings;
        private readonly IServiceBusAdministrator _busAdministrator;
        private ServiceBusClient _client;

        public ServiceBusClientManager(IOptions<ServiceBusSettings> settings, IServiceBusAdministrator busAdministrator)
        {
            _settings = settings.Value;
            _busAdministrator = busAdministrator;
            CreateNewClient();
        }

        private void CreateNewClient()
        {
            _client = new ServiceBusClient(_settings.ConnectionString,
                _settings.ClientOptions ?? new ServiceBusClientOptions());
        }

        public IServiceBusSenderWrapped CreateSender(string queueOrTopicName)
        {
            var sender = _client.CreateSender(queueOrTopicName);

            return new ServiceBusSenderWrapped(sender, _busAdministrator);
        }

        public ServiceBusProcessor CreateProcessor(string name, ServiceBusProcessorOptions options)
        {
            return _client.CreateProcessor(name, options);
        }

        public ServiceBusProcessor CreateProcessor(string name, string subscriptionName, ServiceBusProcessorOptions options)
        {
            return _client.CreateProcessor(name, subscriptionName, options);
        }

        public async Task<ServiceBusSessionReceiver> ReplayToQueue(string queueName, string sessionId)
        {
            try
            {
                return await _client.AcceptSessionAsync(queueName, sessionId);
            }
            catch (ServiceBusException ex) when
                (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
                await using (await AwaitLock.Create(nameof(_busAdministrator.CreateQueueIfNotExistAsync)))
                {
                    await _busAdministrator.CreateQueueIfNotExistAsync(new CreateQueueOptions(queueName)
                        {
                            RequiresSession = true,
                            AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
                        }
                    );
                }

                return await _client.AcceptSessionAsync(queueName, sessionId);
            }
        }

        public IServiceBusClientManager CreateBusClient()
        {
            CreateNewClient();
            return this;
        }
        
        public ValueTask DisposeAsync()
        {
            if (_client is { IsClosed: false })
            {
                return _client.DisposeAsync();
            }
            
            return ValueTask.CompletedTask;
        }
    }

   
}