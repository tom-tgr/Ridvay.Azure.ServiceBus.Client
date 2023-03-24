# Ridvay.Azure.ServiceBus.Client

A simple yet powerful POCO/DTO-driven Azure Service Bus client, Ridvay.Azure.ServiceBus.Client provides an easy-to-use, highly-configurable interface for interacting with Azure Service Bus. With support for sending and receiving messages, registering message consumers, and implementing custom message configurations, this library simplifies the process of integrating Azure Service Bus into your .NET applications.

### Key Features
- Straightforward installation using NuGet
- Effortless registration of Azure Service Bus client and message consumers
- Supports void message consumers and message consumers with reply
- Provides easy message sending, including scheduled messages
- Flexible message configuration with custom attributes
- Manual control over message completion and abandonment

### Getting Started
To start using Ridvay.Azure.ServiceBus.Client in your project, follow the installation and registration steps outlined in the documentation. Once you've set up the library, implement your desired message consumers and begin sending messages with ease.

With the ability to send messages with or without replies, schedule messages for future delivery, and configure message settings, Ridvay.Azure.ServiceBus.Client offers a versatile solution for Azure Service Bus integration. Take advantage of manual message completion and abandonment to have complete control over the processing of your messages.

Conclusion
Ridvay.Azure.ServiceBus.Client is a valuable addition to any .NET project that requires interaction with Azure Service Bus. Its simple yet powerful design allows developers to quickly integrate and manage Azure Service Bus communications, making it an indispensable tool for developers working with Azure Service Bus.

#### Install the package

Install the Ridvay.Azure.ServiceBus.Client library for .NET with NuGet:

```bash
dotnet add package Ridvay.Azure.ServiceBus.Client
```

#### Register services

The given code snippet demonstrates how to register the necessary services for using the Ridvay.Azure.ServiceBus.Client library in your application. This registration process is typically done in the **ConfigureServices** method of the Startup class or a similar configuration class.

Registering the Azure Service Bus Client
First, the Azure Service Bus client is registered using the **AddServiceBusClient** extension method. The method takes a connection string as a parameter, which is retrieved from the application configuration. This connection string is used to connect to your Azure Service Bus instance.

``` csharp
services
.AddServiceBusClient(Configuration.GetConnectionString("ServiceBus"));
```
The **AddServiceBusClient** method is used to register the Azure Service Bus client in your application, which in turn registers the **IMessageSender** interface. The **IMessageSender** interface provides methods for sending messages to the Azure Service Bus, allowing your application to communicate with other services through messages.

When you register the Azure Service Bus client using the **AddServiceBusClient** method, it sets up the necessary infrastructure for sending messages to the appropriate queues or topics. You can then inject the **IMessageSender** interface into your classes and use it to send messages as needed.

##### Registering Message Consumers

Next, you need to register your message consumers, which are responsible for processing the incoming messages. In the example, two message consumers are registered: **MyMessageConsumer** and **MyMessageConsumerWithReturn**. These message consumers are added using the **AddConsumer<T>** extension method.

``` csharp

services
    .AddServiceBusClient(Configuration.GetConnectionString("ServiceBus"))
    .AddConsumer<MyMessageConsumer>()
    .AddConsumer<MyMessageConsumerWithReturn>();
```
The AddConsumer method is used to register worker services that listen for incoming messages on the Azure Service Bus. These worker services, also known as message consumers, process incoming messages and perform the required actions based on the message content.

When you register a message consumer using the **AddConsumer** method, it is configured to listen for messages on the appropriate queue or topic. Once a message arrives, the consumer processes it and takes the necessary action depending on the message type and the consumer's implementation.

##### Ensuring the Correct Order of Service Registration
It is essential to ensure that AddConsumer is registered after AddServiceBusClient in the ConfigureServices method. This ordering is important because it ensures that the Azure Service Bus client is properly registered and configured before the message consumers are registered.



#### Message Consumers Implementation

The Ridvay.Azure.ServiceBus.Client library provides an easy-to-use interface for implementing message consumers. In the given code snippet, two types of message consumers are demonstrated: a void message consumer and a message consumer that can send a reply.

##### Void Message Consumer
The **MyMessageConsumer** class is an example of a void message consumer. It implements the **IMessageConsumer<T>** interface, where T is the type of the message to be consumed (in this case, MyBasicMessage). The ConsumeAsync method is responsible for processing the incoming message.

``` csharp
public class MyMessageConsumer : IMessageConsumer<MyBasicMessage>
{
    public Task ConsumeAsync(IMessageContext<MyBasicMessage> context)
    {
        // Do something with the message
        return Task.CompletedTask;
    }
}
```
In the **ConsumeAsync** method, you can access the message through the IMessageContext<T> parameter, allowing you to read the message's content and properties. After processing the message, return Task.CompletedTask to indicate that the message processing is finished.

##### Message Consumer with Reply
The **MyMessageConsumerWithReturn** class demonstrates a message consumer that can send a reply. It implements the **IMessageConsumerWithReturn<TRequest, TResponse>** interface, where TRequest is the type of the incoming message (MyMessage) and TResponse is the type of the reply message (MyReturnMessage).

``` csharp
public class MyMessageConsumerWithReturn : IMessageConsumerWithReturn<MyMessage, MyReturnMessage>
{
    public MyReturnMessage ConsumeAsync(IMessageContext<MyMessage> context)
    {
        // Do something with the message
        return new MyReturnMessage();
    }
}
```
In the **ConsumeAsync** method, you can access the message through the **IMessageContext<TRequest>** parameter. After processing the incoming message, create an instance of **TResponse** (in this case, **MyReturnMessage**) and return it. The library will take care of sending the reply message back to the sender application.

By implementing these message consumer classes, you can easily handle different types of messages in your application using the Ridvay.Azure.ServiceBus.Client library, including processing messages without expecting a reply and processing messages with a reply using the request-response pattern.

##### Send Messages

The given code snippets demonstrates how
to send messages using Azure Service Bus. The **MyClass** class is an example of how you can implement message sending in your application:

Initialize **IMessageSender**: In the constructor, an instance of **IMessageSender** is injected and stored in the **_sender** field. This instance will be used to send messages throughout the class.
``` csharp
public MyClass(IMessageSender sender)
{
_sender = sender;
}
``` 
**Send a basic message**: The SendMessage method is used to send a basic message without expecting any reply. This method calls the SendAsync method on the _sender instance, passing an instance of **MyBasicMessage**.
``` csharp
public async Task SendMessage()
{
    await _sender.SendAsync(new MyBasicMessage());
}
``` 
**Send a message with a reply**: The SendMessageWithReply method is used when you need to send a message and expect a reply from the receiver. It calls the GetAsync method on the **_sender** instance, specifying the message types for both the request (MyMessage) and the response (MyReturnMessage). The method returns the response as a **MyReturnMessage**.
``` csharp
public async Task<MyReturnMessage> SendMessageWithReply()
{
    return await _sender.GetAsync<MyMessage, MyReturnMessage>(new MyMessage());
}
``` 
**Send a scheduled message**: The ScheduledSendAsync method sends a message that is scheduled to be processed at a specific point in the future. It calls the **ScheduleSendAsync** method on the _sender instance, passing an instance of MyBasicMessage and the desired scheduled processing time (in this example, 15 minutes from the current time).
``` csharp
public async Task ScheduledSendAsync()
{
    await _sender.ScheduleSendAsync(new MyBasicMessage(), DateTime.UtcNow.AddMinutes(15));
}
``` 
By using the Ridvay.Azure.ServiceBus.Client library in this manner, you can easily send messages using Azure Service Bus, send messages with replies using the request-response pattern, and schedule messages for future processing.

``` csharp
using Ridvay.Azure.ServiceBus.Client;
public class MyClass
{
    private readonly IMessageSender _sender;
    public MyClass(IMessageSender sender)
    {
        _sender = sender;
    }

    // Send a message
    public async Task SendMessage()
    {
        await _sender.SendAsync(new MyBasicMessage());
    }

    //Send a message with reply
    public async Task<MyReturnMessage> SendMessageWithReply()
    {
        return await _sender.GetAsync<MyMessage, MyReturnMessage>(new MyMessage());
    }

    // Send a scheduled message
    public async Task ScheduledSendAsync()
    {
        await _sender.ScheduleSendAsync(new MyBasicMessage(), DateTime.UtcNow.AddMinutes(15));
    }
}
```

#### Message configuration
Message configuration plays a vital role in customizing the behavior of message processing in a message-based system, like Azure Service Bus. In the context of Ridvay.Azure.ServiceBus.Client, the QueueConsumer attribute is used to configure the message settings for the message consumer.

The QueueConsumer attribute allows you to customize various aspects of message processing, such as:

**Queue Name**: By setting the QueueName property, you can specify the target queue for the message consumer. This allows you to route messages to different queues based on their type or processing requirements.

``` csharp
[QueueConsumer(QueueName = "SomeQueueName")]
public class MyMessage
{
}
```
**Max Concurrent Calls**: The MaxConcurrentCalls property lets you control the maximum number of concurrent calls to the message consumer. This helps to regulate the load on the consumer and can be adjusted according to your system's resources and performance needs.
``` csharp
[QueueConsumer(MaxConcurrentCalls = 50)]
public class MyMessage
{
}
```
**Prefetch Count**: Prefetching allows the consumer to fetch multiple messages from the queue at once, reducing the number of round-trip requests and improving performance. The PrefetchCount property sets the number of messages to be prefetched by the consumer.
``` csharp
[QueueConsumer(PrefetchCount = 100)]
public class MyMessage
{
}
```
**Auto-Complete Messages**: The AutoCompleteMessages property controls whether the consumer should automatically complete the messages after processing. When set to false, you need to manually complete or abandon the message in the consumer implementation, providing more control over message processing.
``` csharp
[QueueConsumer(AutoCompleteMessages = false)]
public class MyMessage
{
}
```
Similar to the **QueueConsumer** attribute, the **TopicConsumer** attribute allows you to configure message settings for topic-based message processing in Azure Service Bus. The TopicConsumer attribute provides properties for customizing various aspects of message processing, such as:

**Topic Name**: By setting the TopicName property, you can specify the target topic for the message consumer. This allows you to route messages to different topics based on their type or processing requirements.
``` csharp
[TopicConsumer(TopicName = "SomeTopicName")]
public class MyMessage
{
}
```
Subscription Name: The SubscriptionName property allows you to define the subscription name for the message consumer. This enables you to create multiple subscriptions with different filters or processing logic for the same topic.
``` csharp
[TopicConsumer(SubscriptionName = "SomeSubscriptionName")]
public class MyMessage
{
}
```
#### Messages with manual auto completion

 ``` csharp
public class MyMessageConsumerNoAutoComplete : IMessageConsumerWithReturn<MyMessageAutoComplete>
{
    public async Task ConsumeAsync(IMessageContext<MyMessage> context)
    {
        // complete the message manually
        await context.CompleteMessageAsync();
        
        // abandon the message
        await context.AbandonMessageAsync();
    }
}
[QueueConsumer(AutoCompleteMessages = false)]
public class MyMessageAutoComplete
{

}
```

## Request-Response Pattern

The request-response pattern is a well-established integration pattern that enables sender applications to send requests and provides a way for receiver applications to correctly send responses back to the sender. This pattern typically requires a short-lived queue or topic for the sender application to receive responses. In this scenario, Azure Service Bus sessions provide a simple alternative solution with comparable semantics.
Advantages of the Request-Response Pattern over HTTP Requests
Using the request-response pattern instead of HTTP requests offers several advantages in certain scenarios, particularly when designing distributed systems:

**Decoupling**: The request-response pattern decouples sender and receiver components in a distributed system, enabling each component to evolve independently and simplifying maintenance and scaling.

**Reliability**: Azure Service Bus provides built-in mechanisms to ensure message delivery, even in the face of temporary failures, offering more resilience than HTTP requests that may fail due to network issues or server unavailability.

**Asynchronous communication**: The request-response pattern supports asynchronous communication, improving the responsiveness and throughput of your system. Senders can continue processing other tasks while waiting for replies, in contrast to HTTP requests that may block the sender until a response is received.

**Load leveling**: Message-based systems can help distribute workload among multiple instances of the receiver component, allowing for more effective load balancing. This is particularly beneficial when the receiver takes more time to process requests, preventing the sender from becoming a bottleneck.

**Fault tolerance**: By using a message-based system, you can introduce fault tolerance into your application. If a receiver fails, the message can be retried or redirected to another instance, ensuring that the request is eventually processed. In contrast, an HTTP request might fail outright if the server is down.

**Scalability**: The request-response pattern allows you to scale out your system more easily. By adding more instances of the receiving component, you can increase your system's capacity to handle incoming requests. This is more challenging with HTTP-based communication, which often requires load balancers and more complex deployment strategies.

While the request-response pattern offers many advantages over HTTP requests, it is essential to consider your application's specific requirements before choosing the most appropriate communication strategy. 

### Implementation
When implementing the request-response pattern using Azure Service Bus with sessions, the process focuses on enabling reliable, decoupled, and asynchronous communication between sender and receiver applications. The sender application generates a unique message ID and creates a ServiceBusMessage with the serialized payload. By setting the SessionId and ReplyToSessionId properties to the unique message ID, the sender ensures that the response will be delivered to the correct session.

The sender then sends the message to the designated topic or queue and sets up a receiver to listen for responses on the specific session ID. This way, the sender can process the response corresponding to the original request. The sender waits for the response for a specified amount of time, which can be adjusted according to the expected response time.

Upon receiving the response, the sender deserializes the message body and processes it accordingly. This implementation of the request-response pattern with Azure Service Bus allows for a more resilient and scalable communication system compared to traditional HTTP requests, while still providing the benefits of decoupled components and asynchronous processing.