using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Ridvay.Azure.ServiceBus.Client.Abstractions;

// ReSharper disable PossibleNullReferenceException

namespace Ridvay.Azure.ServiceBus.Client.End2End.Test
{
    public class MessageTest : IMessageConsumer<MessageDefault>
    {
        private static IMessageContext<MessageDefault> _receivedMessage;
        private static SemaphoreSlim _messageLock;
        private CancellationTokenSource _cancellationTokenSource;

        private IHost _host;
        private ServiceProvider _services;

        /// <summary>
        ///     Message Handler
        /// </summary>
        public Task ConsumeAsync(IMessageContext<MessageDefault> message)
        {
            _receivedMessage = message;
            _messageLock.Release();

            return Task.CompletedTask;
        }

        [SetUp]
        public async Task Setup()
        {
            _cancellationTokenSource = new CancellationTokenSource();


            _host = Host.CreateDefaultBuilder(new string[] { })
                .ConfigureLogging(builder =>
                    builder.SetMinimumLevel(LogLevel.Warning))
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddServiceBusClient(Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.User))
                        .AddConsumer<MessageTest>();
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
        public async Task Should_Return_Correct_Return_Message()
        {
            _messageLock = new SemaphoreSlim(0, 1);

            RemoveQueue<MessageDefault>();

            var sender = _services.GetService<IMessageSender>();

            var msg = new MessageDefault { TestString = Guid.NewGuid().ToString() };
            await sender.SendAsync(msg);

            await _messageLock.WaitAsync(TimeSpan.FromMinutes(1));


            Assert.IsNotNull(_receivedMessage);

            Assert.AreEqual(msg.TestString, _receivedMessage.Message.TestString);
        }

        [Test]
        public async Task Should_Process_Message_OnTime_Return_Correct_Return_Message()
        {
            _messageLock = new SemaphoreSlim(0, 1);

            RemoveQueue<MessageDefault>();

            var sender = _services.GetService<IMessageSender>();

            var msg = new MessageDefault
            {
                TestString = Guid.NewGuid().ToString()
            };
            var sw = Stopwatch.StartNew();
            var scheduleTime = DateTime.UtcNow.AddMinutes(1);
            await sender.ScheduledSendAsync(msg, scheduleTime);

            await _messageLock.WaitAsync(TimeSpan.FromMinutes(5));
            sw.Stop();

            Assert.AreEqual(1, (int)sw.Elapsed.TotalMinutes);
            Assert.IsNotNull(_receivedMessage);

            Assert.AreEqual(msg.TestString, _receivedMessage.Message.TestString);
        }


        private async void RemoveQueue<T>()
        {
            var administrator = _services.GetRequiredService<IServiceBusAdministrator>();
            await administrator.TryRemoveTopicOrQueueAsync<T>();
        }
    }
}