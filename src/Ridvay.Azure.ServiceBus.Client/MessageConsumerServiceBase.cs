using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Ridvay.Azure.ServiceBus.Client.Abstractions;

namespace Ridvay.Azure.ServiceBus.Client
{
    internal abstract class MessageConsumerServiceBase : IHostedService
    {
        private readonly IServiceBusAdministrator _busAdministrator;
        private ServiceBusProcessor _processor;
        protected readonly IServiceBusClientManager ClientManager;

        internal MessageConsumerServiceBase(
            IServiceBusClientManager clientManagerFactory,
            IServiceBusAdministrator busAdministrator
        )
        {
            _busAdministrator = busAdministrator;
            ClientManager = clientManagerFactory;
        }

        protected abstract bool IsTopic { get; }
        protected abstract string TopicOrQueueName { get; }
        protected abstract ServiceBusProcessorOptions Options { get; }
        protected abstract TopicConsumerAttribute TopicConsumerAttribute { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await CreateProcessor(cancellationToken);

            _processor.ProcessMessageAsync += ProcessorOnProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessorOnProcessErrorAsync;

            await _processor.StartProcessingAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected abstract Task ProcessMessage(ProcessMessageEventArgs args);

        protected virtual Task ProcessMessageError(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }

        private async Task CreateProcessor(CancellationToken cancellationToken)
        {
            if (IsTopic)
                await CreateTopicProcessor(cancellationToken, ClientManager);
            else
                await CreateQueueProcessor(cancellationToken, ClientManager);
        }

        private async Task CreateQueueProcessor(CancellationToken cancellationToken, IServiceBusClientManager clientManager)
        {
            var qName = TopicOrQueueName;

            await _busAdministrator.CreateQueueIfNotExistAsync(qName);

            _processor = clientManager.CreateProcessor(qName, Options);
        }

        private async Task CreateTopicProcessor(CancellationToken cancellationToken, IServiceBusClientManager clientManager)
        {
            var topicConsumerAttribute = TopicConsumerAttribute;
            var qName = TopicOrQueueName;

            await _busAdministrator.CreateTopicIfNotExistsAsync(qName);

            _processor = clientManager.CreateProcessor(qName, topicConsumerAttribute.SubscriptionName, Options);
        }

        private async Task ProcessorOnProcessMessageAsync(ProcessMessageEventArgs args)
        {
            await ProcessMessage(args);
        }

        private Task ProcessorOnProcessErrorAsync(ProcessErrorEventArgs args)
        {
            ProcessMessageError(args);
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        ~MessageConsumerServiceBase()
        {
            _processor.StopProcessingAsync();
        }
    }
}