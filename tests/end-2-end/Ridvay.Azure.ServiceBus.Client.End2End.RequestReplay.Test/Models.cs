using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Ridvay.Azure.ServiceBus.Client.Abstractions;

namespace Ridvay.Azure.ServiceBus.Client.End2End.Test
{
    public class ProduceMessage : IHostedService
    {
        private readonly IMessageSender _sender;

        public ProduceMessage(IMessageSender sender)
        {
            _sender = sender;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)


            {
                var a = await _sender.GetAsync<MessageConcurrent50Prefetch100, BasicMessageResponse>(
                    new MessageConcurrent50Prefetch100() { TestString = Guid.NewGuid().ToString() });
                Thread.Sleep(1000 * 60);
            }

            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask; ;
        }
    }

    [QueueConsumer( MaxConcurrentCalls = 50, PrefetchCount = 100)]
    public class MessageConcurrent50Prefetch100
    {
        public string TestString { get; set; }
    }

    public class MessageDefault
    {
        public string TestString { get; set; }
    }

    public class BasicMessageResponse
    {
        public string ReturnValue { get; set; }
    }

    public class RequestReplayConsumer : 
        IMessageConsumer<MessageConcurrent50Prefetch100, BasicMessageResponse>, 
        IMessageConsumer<MessageDefault, BasicMessageResponse>
    {
        public Task<BasicMessageResponse> ConsumeAsync(IMessageResponse<MessageConcurrent50Prefetch100> response)
        {
            var data = response.Message;


            return Task.FromResult(new BasicMessageResponse { ReturnValue = "OK: "+ data.TestString });
        }

        public Task<BasicMessageResponse> ConsumeAsync(IMessageResponse<MessageDefault> message)
        {
            var data = message.Message;

            return Task.FromResult(new BasicMessageResponse { ReturnValue = "OK: " + data.TestString });
        }
    }

}
