using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Ridvay.Azure.ServiceBus.Client.Abstractions;
using Ridvay.Azure.ServiceBus.Client.Helpers;

namespace Ridvay.Azure.ServiceBus.Client
{
    public static class ServiceBusClientExtensions
    {
        public static ServiceBusClientBuilder AddServiceBus(this IServiceCollection services, string connectionsString)
        {
            return services.AddServiceBus(new ServiceBusSettings() { ConnectionString = connectionsString });
        }

        public static ServiceBusClientBuilder AddServiceBus(this IServiceCollection services, ServiceBusSettings settings)
        {
            services.AddSingleton(settings);
            services.AddSingleton<IMessageSender, MessageSender>();
            services.AddSingleton<IServiceBusAdministrator, ServiceBusAdministrator>();
            services.AddTransient<IServiceBusClientManager, ServiceBusClientManager>();
            services.AddTransient<IConsumerAttributeParserService, ConsumerAttributeParserService>();
            services.AddTransient<IMessageSerialize, MessageSerialize>();
            return new ServiceBusClientBuilder(services);
        }
        /// <summary>
        /// Add Message consumer
        /// </summary>
        /// <typeparam name="TImplementation">Should implement IMessageConsumer<> or IMessageConsumer<,></typeparam>
        public static ServiceBusClientBuilder AddConsumer<TImplementation>(this ServiceBusClientBuilder services)
            where TImplementation : IMessageConsumer
        {

            ValidateConsumerRegistration<TImplementation>(services);

            RegisterRequestReplayTypes<TImplementation>(services);
            RegisterVoidTypes<TImplementation>(services);

            return services;
        }

        private static void ValidateConsumerRegistration<TImplementation>(ServiceBusClientBuilder services)
            where TImplementation : IMessageConsumer
        {
            var items = typeof(TImplementation).GetInterfaces()
                .Where(a => a.IsGenericType &&
                            (a.GetGenericTypeDefinition() == typeof(IMessageConsumer<>) ||
                             a.GetGenericTypeDefinition() == typeof(IMessageConsumer<,>))).ToList();

            if (!items.Any())
                throw new ArgumentException($"Consumer `{typeof(TImplementation).FullName}` should implement IMessageConsumer<> or IMessageConsumer<,>");
            
            items.ForEach(type =>
                {
                    var args = type.GetGenericArguments();
                    var messageType = args[0];

                    var b = services.Where(a =>
                            a.ImplementationType != null &&
                            a.ServiceType == typeof(IHostedService) &&
                            a.ImplementationType.IsAssignableTo(typeof(MessageConsumerServiceBase)))
                        .Select(a => a.ImplementationType).ToList();

                    b.ForEach(a =>
                    {
                        if (a.GetGenericArguments().First() == messageType)
                        {
                            throw new ArgumentException(
                                $"Message type `{messageType.FullName}` already registered in other consumer");
                        }
                    });
                });
        }

        private static void RegisterVoidTypes<TImplementation>(ServiceBusClientBuilder services)
            where TImplementation : IMessageConsumer
        {
            typeof(TImplementation).GetInterfaces()
                .Where(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IMessageConsumer<>))
                .ToList()
                .ForEach(type =>
                {
                    var args = type.GetGenericArguments();
                    var typeToMake = typeof(MessageConsumerVoidMessageService<>);
                    var constructedConsumer = typeToMake.MakeGenericType(args);
                    
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), constructedConsumer));
                    services.TryAddTransient(type, typeof(TImplementation));
                });
        }

        private static void RegisterRequestReplayTypes<TImplementation>(ServiceBusClientBuilder services)
            where TImplementation : IMessageConsumer
        {
            typeof(TImplementation).GetInterfaces()
                .Where(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IMessageConsumer<,>))
                .ToList()
                .ForEach(type =>
                {
                    var args = type.GetGenericArguments();
                    var typeToMake = typeof(MessageConsumerRequestReplayMessageService<,>);
                    var constructedConsumer = typeToMake.MakeGenericType(args);
                    
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), constructedConsumer));
                    services.TryAddTransient(type, typeof(TImplementation));
                });
        }
    }
}