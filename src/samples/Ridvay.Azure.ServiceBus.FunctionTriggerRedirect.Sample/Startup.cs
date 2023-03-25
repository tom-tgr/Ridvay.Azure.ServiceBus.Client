using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Ridvay.Azure.ServiceBus.Client;
using Ridvay.Azure.ServiceBus.Client.Abstractions.FunctionTrigger;
using Ridvay.Azure.ServiceBus.FunctionTriggerRedirect.Sample;
using Ridvay.Azure.ServiceBus.MessageModels.Samples;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Ridvay.Azure.ServiceBus.FunctionTriggerRedirect.Sample;

[FunctionTimerTrigger("DailyUserProfileUpdater", "0 0 * * *", typeof(BasicMessage))]
[FunctionTimerTrigger("Every15thMinute", "*/15 * * * *", typeof(BasicMessage))]
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.User);
        builder.Services.AddServiceBusClient(serviceBusConnectionString);
    }
}

