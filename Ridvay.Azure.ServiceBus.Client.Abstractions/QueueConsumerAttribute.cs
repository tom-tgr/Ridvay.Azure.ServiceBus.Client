namespace Ridvay.Azure.ServiceBus.Client.Abstractions
{
    public class QueueConsumerAttribute : BusConsumerAttribute
    {
        public string QueueName { get; set; }
    }
}