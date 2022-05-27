using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Ridvay.Azure.ServiceBus.Client.Abstractions;

namespace Ridvay.Azure.ServiceBus.Client
{
    public class MessageContext<T> : IMessageContext<T>, IMessageDetails

    {
        private readonly ProcessMessageEventArgs _processMessageEventArgs;


        public MessageContext(T message, ProcessMessageEventArgs  processMessageEventArgs)
        {
            _processMessageEventArgs = processMessageEventArgs;
            Message = message;
        }

        /// <summary>
        /// The message
        /// </summary>
        public T Message { get; }

        /// <summary>
        /// Message Details
        /// </summary>
        public IMessageDetails MessageDetails => this;


        ///<inheritdoc cref="ServiceBusReceivedMessage.MessageId"/>
        public string MessageId => _processMessageEventArgs.Message.MessageId;

        ///<inheritdoc cref="ServiceBusReceivedMessage.PartitionKey"/>
        public string PartitionKey => _processMessageEventArgs.Message.PartitionKey;

        ///<inheritdoc cref="ServiceBusReceivedMessage.TransactionPartitionKey"/>
        public string TransactionPartitionKey => _processMessageEventArgs.Message.TransactionPartitionKey;

        ///<inheritdoc cref="ServiceBusReceivedMessage.SessionId"/>
        public string SessionId => _processMessageEventArgs.Message.SessionId;

        ///<inheritdoc cref="ServiceBusReceivedMessage.ReplyToSessionId"/>
        public string ReplyToSessionId => _processMessageEventArgs.Message.ReplyToSessionId;

        ///<inheritdoc cref="ServiceBusReceivedMessage.TimeToLive"/>
        public TimeSpan TimeToLive => _processMessageEventArgs.Message.TimeToLive;

        ///<inheritdoc cref="ServiceBusReceivedMessage.CorrelationId"/>
        public string CorrelationId => _processMessageEventArgs.Message.CorrelationId;

        ///<inheritdoc cref="ServiceBusReceivedMessage.Subject"/>
        public string Subject => _processMessageEventArgs.Message.Subject;

        ///<inheritdoc cref="ServiceBusReceivedMessage.To"/>
        public string To => _processMessageEventArgs.Message.To;

        ///<inheritdoc cref="ServiceBusReceivedMessage.ContentType"/>
        public string ContentType => _processMessageEventArgs.Message.ContentType;

        ///<inheritdoc cref="ServiceBusReceivedMessage.ReplyTo"/>
        public string ReplyTo => _processMessageEventArgs.Message.ReplyTo;

        ///<inheritdoc cref="ServiceBusReceivedMessage.ScheduledEnqueueTime"/>
        public DateTimeOffset ScheduledEnqueueTime => _processMessageEventArgs.Message.ScheduledEnqueueTime;

        ///<inheritdoc cref="ServiceBusReceivedMessage.ApplicationProperties"/>
        public IReadOnlyDictionary<string, object> ApplicationProperties => _processMessageEventArgs.Message.ApplicationProperties;

        ///<inheritdoc cref="ServiceBusReceivedMessage.LockToken"/>
        public string LockToken => _processMessageEventArgs.Message.LockToken;

        ///<inheritdoc cref="ServiceBusReceivedMessage.DeliveryCount"/>
        public int DeliveryCount => _processMessageEventArgs.Message.DeliveryCount;

        ///<inheritdoc cref="ServiceBusReceivedMessage.LockedUntil"/>
        public DateTimeOffset LockedUntil => _processMessageEventArgs.Message.LockedUntil;

        ///<inheritdoc cref="ServiceBusReceivedMessage.SequenceNumber"/>
        public long SequenceNumber => _processMessageEventArgs.Message.SequenceNumber;

        ///<inheritdoc cref="ServiceBusReceivedMessage.DeadLetterSource"/>
        public string DeadLetterSource => _processMessageEventArgs.Message.DeadLetterSource;

        ///<inheritdoc cref="ServiceBusReceivedMessage.EnqueuedSequenceNumber"/>
        public long EnqueuedSequenceNumber => _processMessageEventArgs.Message.EnqueuedSequenceNumber;

        ///<inheritdoc cref="ServiceBusReceivedMessage.EnqueuedTime"/>
        public DateTimeOffset EnqueuedTime => _processMessageEventArgs.Message.EnqueuedTime;

        ///<inheritdoc cref="ServiceBusReceivedMessage.ExpiresAt"/>
        public DateTimeOffset ExpiresAt => _processMessageEventArgs.Message.ExpiresAt;

        ///<inheritdoc cref="ServiceBusReceivedMessage.DeadLetterReason"/>
        public string DeadLetterReason => _processMessageEventArgs.Message.DeadLetterReason;

        ///<inheritdoc cref="ServiceBusReceivedMessage.DeadLetterErrorDescription"/>
        public string DeadLetterErrorDescription => _processMessageEventArgs.Message.DeadLetterErrorDescription;

        ///<inheritdoc cref="ServiceBusReceivedMessage.State"/>
        public string State => _processMessageEventArgs.Message.State.ToString();


        ///<inheritdoc cref="ServiceBusReceiver.AbandonMessageAsync(ServiceBusReceivedMessage, IDictionary{string, object}, CancellationToken)"/>
        public Task AbandonMessageAsync(IDictionary<string, object> propertiesToModify = default,
            CancellationToken cancellationToken = default) =>
            _processMessageEventArgs.AbandonMessageAsync(_processMessageEventArgs.Message, propertiesToModify, cancellationToken);

        ///<inheritdoc cref="ServiceBusReceiver.CompleteMessageAsync(ServiceBusReceivedMessage, CancellationToken)"/>
        public Task CompleteMessageAsync(CancellationToken cancellationToken = default) =>
            _processMessageEventArgs.CompleteMessageAsync(_processMessageEventArgs.Message,  cancellationToken);

        ///<inheritdoc cref="ServiceBusReceiver.DeadLetterMessageAsync(ServiceBusReceivedMessage, string, string, CancellationToken)"/>
        public Task DeadLetterMessageAsync(string deadLetterReason,
            string deadLetterErrorDescription = default,
            CancellationToken cancellationToken = default) =>
            _processMessageEventArgs.DeadLetterMessageAsync(_processMessageEventArgs.Message, deadLetterReason, deadLetterErrorDescription, cancellationToken);

        ///<inheritdoc cref="ServiceBusReceiver.DeadLetterMessageAsync(ServiceBusReceivedMessage, IDictionary{string, object}, CancellationToken)"/>
        public Task DeadLetterMessageAsync(IDictionary<string, object> propertiesToModify = default,
            CancellationToken cancellationToken = default) =>
            _processMessageEventArgs.DeadLetterMessageAsync(_processMessageEventArgs.Message, propertiesToModify, cancellationToken);

        ///<inheritdoc cref="ServiceBusReceiver.DeferMessageAsync(ServiceBusReceivedMessage, IDictionary{string, object}, CancellationToken)"/>
        public Task DeferMessageAsync(IDictionary<string, object> propertiesToModify = default,
            CancellationToken cancellationToken = default) =>
            _processMessageEventArgs.DeferMessageAsync(_processMessageEventArgs.Message, propertiesToModify, cancellationToken);

        ///<inheritdoc cref="ServiceBusReceiver.RenewMessageLockAsync(ServiceBusReceivedMessage, CancellationToken)"/>
        public Task RenewMessageLockAsync(CancellationToken cancellationToken = default) =>
            _processMessageEventArgs.RenewMessageLockAsync(_processMessageEventArgs.Message, cancellationToken);

    }
}
