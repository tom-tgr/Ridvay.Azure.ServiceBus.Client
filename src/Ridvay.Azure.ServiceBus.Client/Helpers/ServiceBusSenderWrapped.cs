using System;
using System.Threading.Tasks;
using AsyncKeyedLock;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Ridvay.Azure.ServiceBus.Client.Helpers;

namespace Ridvay.Azure.ServiceBus.Client
{
    public interface IServiceBusSenderWrapped
    {
        Task SendMessageAsync<T>(ServiceBusMessage message);
        Task ReplayMessage(ServiceBusMessage message, string queueName);
    }

    internal class ServiceBusSenderWrapped : IServiceBusSenderWrapped
    {
        private readonly ServiceBusSender _sender;
        private readonly IServiceBusAdministrator _busAdministrator;
        private readonly AsyncKeyedLocker<string> _asyncKeyedLocker;

        public ServiceBusSenderWrapped(ServiceBusSender sender, IServiceBusAdministrator busAdministrator, AsyncKeyedLocker<string> asyncKeyedLocker)
        {
            _sender = sender;
            _busAdministrator = busAdministrator;
            _asyncKeyedLocker = asyncKeyedLocker;
        }

        
        public async Task SendMessageAsync<T>(ServiceBusMessage message)
        {
            try
            {
                await _sender.SendMessageAsync(message);
            }
            catch (ServiceBusException ex) when 
                (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
                await _busAdministrator.CreateTopicOrQueueIfNotExistsAsync<T>();
                await _sender.SendMessageAsync(message);
            }
        }
        public async Task ReplayMessage(ServiceBusMessage message, string queueName)
        {
            try
            {
                await _sender.SendMessageAsync(message);
            }
            catch (ServiceBusException ex) when
                (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
                using (await _asyncKeyedLocker.LockAsync(nameof(_busAdministrator.CreateQueueIfNotExistAsync)).ConfigureAwait(false))
                {
                    await _busAdministrator.CreateQueueIfNotExistAsync(new CreateQueueOptions(queueName)
                    {
                        RequiresSession = true,
                        AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
                    });
                }

                await _sender.SendMessageAsync(message);
            }
        }

    }
}