using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Ridvay.Azure.ServiceBus.Client
{
    public interface IServiceBusClientManager
    {
        IServiceBusClientManager CreateBusClient();
        IServiceBusSenderWrapped CreateSender(string queueOrTopicName);
        ServiceBusProcessor CreateProcessor(string name, ServiceBusProcessorOptions options);
        ServiceBusProcessor CreateProcessor(string name, string subscriptionName, ServiceBusProcessorOptions options);
        Task<ServiceBusSessionReceiver> ReplayToQueue(string queueName, string sessionId);
    }
}