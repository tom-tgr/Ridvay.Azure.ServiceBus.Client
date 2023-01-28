using System;
using System.Threading.Tasks;

namespace Ridvay.Azure.ServiceBus.Client
{
    public interface IMessageSender
    {
        /// <summary>
        ///     Send Message to
        /// </summary>
        Task SendAsync<T>(T msg);

        /// <summary>
        ///     You can submit messages to a queue or topic for delayed processing;
        ///     for example, to schedule a job to become available for processing by a system at a certain time.
        ///     This capability realizes a reliable distributed time-based scheduler.
        /// </summary>
        Task ScheduledSendAsync<T>(T msg, DateTime scheduledEnqueueTimeUtc);


        /// <summary>
        ///     Request-Reply Pattern
        /// </summary>
        /// <remarks>
        ///     Sends Message to consumer<T> and waits for response from message consumer
        /// </remarks>
        Task<TResponse> GetAsync<T, TResponse>(T msg);
    }
}