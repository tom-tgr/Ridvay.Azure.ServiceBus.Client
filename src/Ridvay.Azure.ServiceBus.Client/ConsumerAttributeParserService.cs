using System;
using System.Linq;
using Azure.Messaging.ServiceBus;
using Ridvay.Azure.ServiceBus.Client.Abstractions;
using AzureServiceBusReceiveMode = Azure.Messaging.ServiceBus.ServiceBusReceiveMode;
using AzureSubQueue = Azure.Messaging.ServiceBus.SubQueue;

namespace Ridvay.Azure.ServiceBus.Client
{
    public interface IConsumerAttributeParserService
    {
        ServiceBusProcessorOptions GetOptions<T>();
        bool IsTopicProcessor<T>();
        TopicConsumerAttribute GetTopicConsumerAttribute<T>();
        QueueConsumerAttribute GetQueueConsumerAttribute<T>();
        string GetTopicOrQueueName<T>();
        string GetReplayQueueName<T>();
    }

    public class ConsumerAttributeParserService : IConsumerAttributeParserService
    {
        public ServiceBusProcessorOptions GetOptions<T>()
        {
            var retValue = new ServiceBusProcessorOptions();
            var type = typeof(T);

            var item = (BusConsumerAttribute)type.GetCustomAttributes(typeof(BusConsumerAttribute), true).FirstOrDefault();

            if (item != null)
            {
                Enum.TryParse(item.ReceiveMode.ToString(), out AzureServiceBusReceiveMode receiveMode);
                Enum.TryParse(item.ReceiveMode.ToString(), out AzureSubQueue subQueue);

                return new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = item.AutoCompleteMessages,
                    MaxAutoLockRenewalDuration = item.MaxAutoLockRenewalDuration,
                    MaxConcurrentCalls = item.MaxConcurrentCalls,
                    PrefetchCount = item.PrefetchCount,
                    ReceiveMode = receiveMode,
                    SubQueue = subQueue
                };
            }

            return retValue;
        }

        public bool IsTopicProcessor<T>()
        {
            return typeof(T)
                .GetCustomAttributes(typeof(TopicConsumerAttribute), true)
                .Any();
        }

        public TopicConsumerAttribute GetTopicConsumerAttribute<T>()
        {
            var consumerAttribute = (TopicConsumerAttribute)typeof(T)
                .GetCustomAttributes(typeof(TopicConsumerAttribute), true)
                .FirstOrDefault();

            if (consumerAttribute == null)
                throw new ArgumentNullException("TopicConsumerAttribute");

            if (string.IsNullOrEmpty(consumerAttribute.SubscriptionName))
                throw new ArgumentNullException("TopicConsumerAttribute.SubscriptionName");

            if (string.IsNullOrEmpty(consumerAttribute.TopicName))
                throw new ArgumentNullException("TopicConsumerAttribute.TopicName");
            return consumerAttribute;
        }

        public QueueConsumerAttribute GetQueueConsumerAttribute<T>()
        {
            var consumerAttribute = (QueueConsumerAttribute)typeof(T)
                .GetCustomAttributes(typeof(QueueConsumerAttribute), true)
                .FirstOrDefault();

            return consumerAttribute;
        }

        public string GetTopicOrQueueName<T>()
        {
            var result = IsTopicProcessor<T>() ? GetTopicConsumerAttribute<T>()?.TopicName : GetQueueConsumerAttribute<T>()?.QueueName;

            return result ?? $"{typeof(T).Namespace}.{typeof(T).Name}";
        }

        public string GetReplayQueueName<T>()
        {
            return GetTopicOrQueueName<T>() + "____Replay";
        }
    }
}