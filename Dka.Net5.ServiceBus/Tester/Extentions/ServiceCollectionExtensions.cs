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
using ServiceBusTester.Models;
using ServiceBusTester.Models.Events.V1.Tenant;
using ServiceBusTester.Models.Events.V1.User;
using ServiceBusTester.Models.Responses;

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
                    DeserializationDictionary = new Dictionary<(int, string), Type>
                    {
                        { (1, AppConstants.Events.TenantEvents.TenantUpdated.FullName), typeof(ServiceBusMessage<TenantUpdated>) },
                        { (1, AppConstants.Events.Responses.GeneralResponse.FullName), typeof(ServiceBusMessage<Response>) },
                        { (1, AppConstants.Events.UserEvents.UserUpdated.FullName), typeof(ServiceBusMessage<UserUpdated>) }
                    }
                });

            services.AddScoped<IServiceBusPublisher, ServiceBusPublisher.ServiceBusPublisher>();
            services.AddScoped<IServiceBusSubscriber, ServiceBusSubscriber.ServiceBusSubscriber>();

            var sp = services.BuildServiceProvider();
            var serviceBusPublisher = sp.GetRequiredService<IServiceBusPublisher>();
            var serviceBusSubscriber = sp.GetRequiredService<IServiceBusSubscriber>();

            foreach (var topic in AppConstants.ServiceBus.Publish.Queues)
            {
                serviceBusPublisher.EnsureQueue(topic, CancellationToken.None);
            }
            
            foreach (var topic in AppConstants.ServiceBus.Receive.Queues)
            {
                serviceBusPublisher.EnsureQueue(topic, CancellationToken.None);
            }
            
            foreach (var topic in AppConstants.ServiceBus.Publish.Topics)
            {
                serviceBusPublisher.EnsureTopic(topic, CancellationToken.None);
            }
            
            foreach (var topic in AppConstants.ServiceBus.Receive.Topics)
            {
                serviceBusPublisher.EnsureTopic(topic, CancellationToken.None);
            }

            foreach (var (topic, subscription) in AppConstants.ServiceBus.Receive.TopicSubscriptions)
            {
                serviceBusSubscriber.EnsureTopicSubscription(topic, subscription, CancellationToken.None);
            }            
            
            services.AddScoped<ITalentechAdminServiceBusClient, TalentechAdminServiceBusClient>();
            
            return services;
        }
    }
}