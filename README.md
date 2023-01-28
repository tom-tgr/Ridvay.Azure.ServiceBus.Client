# Ridvay.Azure.ServiceBus.Client

Simple POCO/DTO driven Azure Service Bus client

## Install the package

Install the Ridvay.Azure.ServiceBus.Client library for .NET with NuGet:

```bash
dotnet add package Ridvay.Azure.ServiceBus.Client
```

## Register services

``` csharp
using Ridvay.Azure.ServiceBus.Client;
public void ConfigureServices(IServiceCollection services)
{
    services
    //Register the Azure Service Bus client
    .AddServiceBusClient(Configuration.GetConnectionString("ServiceBus"))
    //Register message consumers
    .AddConsumer<MyMessageConsumer>()
    .AddConsumer<MyMessageConsumerWithReturn>()
    ;
};
  
```

## Mesage Consumers Implemention

#

### Void message consumer

``` csharp

public class MyMessageConsumer : IMessageConsumer<MyBasicMessage>
{
    public Task ConsumeAsync(IMessageContext<MyBasicMessage> context)
    {
        //Do something with the message
        return Task.CompletedTask;
    }
}

```

### Message with reply consumer

``` csharp
public class MyMessageConsumerWithReturn : IMessageConsumerWithReturn<MyMessage, MyReturnMessage>
{
    public MyReturnMessage ConsumeAsync(IMessageContext<MyMessage> context)
    {
        //Do something with the message
        return new MyReturnMessage();
    }
}
``` 

## Send Messages

#

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

## Message configuration

#

``` csharp  

// Default configuration
public class MyBasicMessage
{

}

// Custom configuration
[QueueConsumer(QueueName= "SomeQueueName", MaxConcurrentCalls = 50, PrefetchCount = 100, AutoCompleteMessages = false)]
public class MyMessage
{

}
```

### Messages with manual auto completion

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

```
