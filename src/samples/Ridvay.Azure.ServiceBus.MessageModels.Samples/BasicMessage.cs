using Ridvay.Azure.ServiceBus.Client.Abstractions;

namespace Ridvay.Azure.ServiceBus.MessageModels.Samples;

[QueueConsumer]
public class BasicMessage
{
    public string SomeProperty { get; set; } = string.Empty;
}
