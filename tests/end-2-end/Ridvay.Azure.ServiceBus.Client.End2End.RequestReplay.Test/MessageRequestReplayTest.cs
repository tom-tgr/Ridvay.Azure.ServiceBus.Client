using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

// ReSharper disable PossibleNullReferenceException

namespace Ridvay.Azure.ServiceBus.Client.End2End.Test
{
    public class MessageRequestReplayTest
    {
        private ServiceProvider _services;

        private readonly object _lock =new object();
        private IHost _host;
        private CancellationTokenSource _cancellationTokenSource;

        [SetUp]
        public async Task Setup()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            

            _host = Host.CreateDefaultBuilder(new string[]{})
                .ConfigureLogging(builder =>
                    builder.SetMinimumLevel(LogLevel.Warning))
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.User))
                        .AddConsumer<RequestReplayConsumer>();
                    _services = services.BuildServiceProvider();
                }).Build();
               
            await _host.StartAsync(_cancellationTokenSource.Token);
        }

        [TearDown]
        public async Task TearDown()
        {
            //Not working
            _cancellationTokenSource.Cancel();
            //Not working
            await _host.StopAsync();

            var lf = _services.GetRequiredService<IHostApplicationLifetime>();
            lf.StopApplication();
            
        }

        [Test]
        public async Task Massage_Should_Return_Value()
        {
            var sender = _services.GetService<IMessageSender>();
            var stringValue = Guid.NewGuid().ToString(); 
            
            var a = await sender.GetAsync<MessageDefault, BasicMessageResponse>(new MessageDefault() { TestString = stringValue });

            Assert.AreEqual("OK: "+ stringValue, a.ReturnValue);
        }

        [Test]
        public async Task Should_Return_Correct_Return_Message()
        {
            var sender = _services.GetService<IMessageSender>();

            var results = new List<BasicMessageResponse>();
        
            var tasks = Enumerable.Range(0, 100)
                .Select(i => Task.Run(async () =>
                {
                    var a = await sender.GetAsync<MessageConcurrent50Prefetch100, BasicMessageResponse>(new MessageConcurrent50Prefetch100() { TestString = i.ToString() });

                    lock(_lock) 
                        results.Add(a);

                    Assert.AreEqual("OK: " + i, a.ReturnValue);
                }))
                .ToList();

            await Task.WhenAll(tasks);

            
            Assert.AreEqual(100, results.Count);

        }
    }
}
