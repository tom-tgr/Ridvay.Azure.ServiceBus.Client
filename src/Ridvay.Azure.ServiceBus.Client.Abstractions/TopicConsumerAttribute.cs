using System;

namespace Ridvay.Azure.ServiceBus.Client.Abstractions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TopicConsumerAttribute : Attribute
    {
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
    }
}