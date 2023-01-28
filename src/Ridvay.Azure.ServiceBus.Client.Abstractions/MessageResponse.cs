using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ridvay.Azure.ServiceBus.Client.Abstractions
{
    public interface IMessageContext<T>
    {
        /// <summary>
        ///     The message
        /// </summary>
        T Message { get; }

        /// <summary>
        ///     Message Details
        /// </summary>
        IMessageDetails MessageDetails { get; }

        /// <inheritdoc cref="ServiceBusReceiver.AbandonMessageAsync(ServiceBusReceivedMessage, IDictionary{string, object}, CancellationToken)" />
        Task AbandonMessageAsync(IDictionary<string, object> propertiesToModify = default,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="ServiceBusReceiver.CompleteMessageAsync(ServiceBusReceivedMessage, CancellationToken)" />
        Task CompleteMessageAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc cref="ServiceBusReceiver.DeadLetterMessageAsync(ServiceBusReceivedMessage, string, string, CancellationToken)" />
        Task DeadLetterMessageAsync(string deadLetterReason,
            string deadLetterErrorDescription = default,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="ServiceBusReceiver.DeadLetterMessageAsync(ServiceBusReceivedMessage, IDictionary{string, object}, CancellationToken)" />
        Task DeadLetterMessageAsync(IDictionary<string, object> propertiesToModify = default,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="ServiceBusReceiver.DeferMessageAsync(ServiceBusReceivedMessage, IDictionary{string, object}, CancellationToken)" />
        Task DeferMessageAsync(IDictionary<string, object> propertiesToModify = default,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="ServiceBusReceiver.RenewMessageLockAsync(ServiceBusReceivedMessage, CancellationToken)" />
        Task RenewMessageLockAsync(CancellationToken cancellationToken = default);
    }

    public interface IMessageDetails
    {
        /// <inheritdoc cref="ServiceBusReceivedMessage.MessageId" />
        string MessageId { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.PartitionKey" />
        string PartitionKey { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.TransactionPartitionKey" />
        string TransactionPartitionKey { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.SessionId" />
        string SessionId { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.ReplyToSessionId" />
        string ReplyToSessionId { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.TimeToLive" />
        TimeSpan TimeToLive { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.CorrelationId" />
        string CorrelationId { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.Subject" />
        string Subject { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.To" />
        string To { get; }

        /// <inheritdoc cref="System.Net.Mime.ContentType" />
        string ContentType { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.ReplyTo" />
        string ReplyTo { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.ScheduledEnqueueTime" />
        DateTimeOffset ScheduledEnqueueTime { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.ApplicationProperties" />
        IReadOnlyDictionary<string, object> ApplicationProperties { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.LockToken" />
        string LockToken { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.DeliveryCount" />
        int DeliveryCount { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.LockedUntil" />
        DateTimeOffset LockedUntil { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.SequenceNumber" />
        long SequenceNumber { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.DeadLetterSource" />
        string DeadLetterSource { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.EnqueuedSequenceNumber" />
        long EnqueuedSequenceNumber { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.EnqueuedTime" />
        DateTimeOffset EnqueuedTime { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.ExpiresAt" />
        DateTimeOffset ExpiresAt { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.DeadLetterReason" />
        string DeadLetterReason { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.DeadLetterErrorDescription" />
        string DeadLetterErrorDescription { get; }

        /// <inheritdoc cref="ServiceBusReceivedMessage.State" />
        string State { get; }
    }
}