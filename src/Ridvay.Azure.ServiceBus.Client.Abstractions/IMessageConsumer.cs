using System.Threading.Tasks;

namespace Ridvay.Azure.ServiceBus.Client.Abstractions
{
    public interface IMessageConsumer
    {
    }
    public interface IMessageConsumer<TRequest> : IMessageConsumer where TRequest : class 
    {
        public Task ConsumeAsync(IMessageContext<TRequest> message);
    }

    public interface IMessageConsumer<TRequest, TReplay> : IMessageConsumer where TRequest : class
    {
        public Task<TReplay> ConsumeAsync(IMessageContext<TRequest> message);
    }
}