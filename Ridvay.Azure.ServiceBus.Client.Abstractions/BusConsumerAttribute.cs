using System;

namespace Ridvay.Azure.ServiceBus.Client.Abstractions
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class BusConsumerAttribute : Attribute
    {
        
        /// <summary>
        /// Gets or sets the number of messages that will be eagerly requested
        /// from Queues or SubscriptionName and queued locally, intended to help
        /// maximize throughput by allowing the processor to receive
        /// from a local cache rather than waiting on a service request.
        /// </summary>
        /// <value>The default value is 0.</value>
        public int PrefetchCount { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the processor
        /// should automatically complete messages after the <see cref="ServiceBusProcessor.ProcessMessageAsync"/> handler has
        /// completed processing. If the message handler triggers an exception, the message will not be automatically completed.
        /// </summary>
        /// <remarks>
        /// If the message handler triggers an exception and did not settle the message,
        /// then the message will be automatically abandoned, irrespective of <see cref= "AutoCompleteMessages" />.
        /// </remarks>
        ///
        /// <value><c>true</c> to complete the message automatically on successful execution of the message handler; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.</value>
        public bool AutoCompleteMessages { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="ReceiveMode"/> used to specify how messages
        /// are received.
        /// </summary>
        ///
        /// <value>The mode to use for receiving messages. The default value is <see cref="ServiceBusReceiveMode.PeekLock"/>.</value>
        public ServiceBusReceiveMode ReceiveMode { get; set; } = ServiceBusReceiveMode.PeekLock;


        /// <summary>
        /// Gets or sets the maximum duration within which the lock will be renewed automatically. This
        /// value should be greater than the longest message lock duration; for example, the LockDuration Property.
        /// </summary>
        public TimeSpan MaxAutoLockRenewalDuration { get; set; } = TimeSpan.FromMinutes(5);



        /// <summary>Gets or sets the maximum number of concurrent calls to the
        /// message handler the processor should initiate.
        /// </summary>
        public int MaxConcurrentCalls { get; set; }

        /// <summary>
        /// Gets or sets the subqueue to connect the processor to.
        /// </summary>
        ///
        /// <value>The subqueue to connect the processor to. The default value is <see cref="SubQueue.None"/>, meaning the processor will
        /// not connect to a subqueue.
        /// </value>
        public SubQueue SubQueue { get; set; } = SubQueue.None;
    }

    /// <summary>
    /// Represents the possible system subqueues that can be received from.
    /// </summary>
    public enum SubQueue
    {
        /// <summary>
        /// No subqueue, the queue itself will be received from.
        /// </summary>
        None = 0,

        /// <summary>
        /// The dead-letter subqueue contains messages that have been dead-lettered.
        /// <see href="https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dead-letter-queues#moving-messages-to-the-dlq"/>
        /// </summary>
        DeadLetter = 1,

        /// <summary>
        /// The transfer dead-letter subqueue contains messages that have been dead-lettered when
        /// the following conditions apply:
        /// <list type="bullet">
        /// <item>
        /// <description>A message passes through more than four queues or topics that are chained together.</description>
        /// </item>
        /// <item>
        /// <description>The destination queue or topic is disabled or deleted.</description>
        /// </item>
        /// <item>
        /// <description>The destination queue or topic exceeds the maximum entity size.</description>
        /// </item>
        /// </list>
        /// <seealso href="https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dead-letter-queues#dead-lettering-in-forwardto-or-sendvia-scenarios"/>
        /// </summary>
        TransferDeadLetter = 2
    }

    /// <summary>
    /// The mode in which to receive messages.
    /// </summary>
    public enum ServiceBusReceiveMode
    {
        /// <summary>
        /// Once a message is received in this mode, the receiver has a lock on the message for a
        /// particular duration. If the message is not settled by this time, it lands back on Service Bus
        /// to be fetched by the next receive operation.
        /// </summary>
        ///
        /// <remarks>This is the default value for <see cref="ServiceBusReceiveMode" />, and should be used for guaranteed delivery.</remarks>
        PeekLock,

        /// <summary>
        /// ReceiveAndDelete will delete the message from Service Bus as soon as the message is delivered.
        /// </summary>
        ReceiveAndDelete,
    }
}