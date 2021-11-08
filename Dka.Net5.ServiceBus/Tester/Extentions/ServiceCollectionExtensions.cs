using System;
using System.Collections.Generic;
using System.Threading;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusMessages;
using ServiceBusPublisher;
using ServiceBusSubscriber;
using ServiceBusTester.Logic;
using ServiceBusTester.Models.Events.V1.Tenant;
using ServiceBusTester.Models.Events.V1.User;

namespace ServiceBusTester.Extentions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new ServiceBusClient(configuration["ServiceBus:ConnectionString"]));
            services.AddSingleton(new ServiceBusAdministrationClient(configuration["ServiceBus:ConnectionString"]));
            services.AddSingleton(
                new ServiceBusSubscriberConfiguration
                {
                    DeserializationDictionary = new Dictionary<string, Type>
                    {
                        { nameof(TenantUpdated), typeof(ServiceBusMessage<TenantUpdated>) },
                        { nameof(UserUpdated), typeof(ServiceBusMessage<UserUpdated>) }
                    }
                });

            services.AddScoped<IServiceBusPublisher, ServiceBusPublisher.ServiceBusPublisher>();
            services.AddScoped<IServiceBusSubscriber, ServiceBusSubscriber.ServiceBusSubscriber>();

            services.AddScoped<ITalentechAdminServiceBusClient, TalentechAdminServiceBusClient>();
            
            var sp = services.BuildServiceProvider();
            var serviceBusPublisher = sp.GetRequiredService<IServiceBusPublisher>();
            var serviceBusSubscriber = sp.GetRequiredService<IServiceBusSubscriber>();

            serviceBusPublisher.EnsureTopic(configuration["ServiceBus:Topic"], CancellationToken.None);
            serviceBusSubscriber.EnsureTopicSubscription(configuration["ServiceBus:Topic"], configuration["ServiceBus:Subscription"], CancellationToken.None);
            
            return services;
        }
    }
}