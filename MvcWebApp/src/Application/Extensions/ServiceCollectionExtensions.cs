using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Application.Logic.ServiceBus;
using Application.Mapping;
using Application.Models;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Infrastructure;
using Infrastructure.Mapping;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;
using ServiceBusPublisher;
using ServiceBusSubscriber;

// TODO: Move all these extensions to Web project. This is not connected to Application.

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddInfrastructure();
            services.SetupInfrastructure();
            
            services.AddAutoMapper(typeof(ApplicationProfile));
            services.AddMediatR(Assembly.GetExecutingAssembly());
            
            return services;
        }

        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(
                new ServiceBusSubscriberConfiguration
                {
                    DeserializationDictionary = new Dictionary<(int, string), Type>
                    {
                    }
                });

            var serviceBusConnectionString =
                configuration[ApplicationConstants.ServiceBus.Configuration.ConnnectionStringPath];

            if (!string.IsNullOrWhiteSpace(serviceBusConnectionString))
            {
                services.TryAddSingleton(new ServiceBusClient(serviceBusConnectionString));
                services.TryAddSingleton(new ServiceBusAdministrationClient(serviceBusConnectionString));

                services.AddScoped<IServiceBusPublisher, ServiceBusPublisher.ServiceBusPublisher>();
                services.AddScoped<IServiceBusSubscriber, ServiceBusSubscriber.ServiceBusSubscriber>();
                services.AddScoped<IServiceBusSubscriberForDeadLetter, ServiceBusSubscriber.ServiceBusSubscriber>();
            }
            else
            {
                services.AddScoped<IServiceBusPublisher, LocalhostServiceBusPublisher>();
                services.AddScoped<IServiceBusSubscriber, LocalhostServiceBusSubscriber>();
                services.AddScoped<IServiceBusSubscriberForDeadLetter, LocalhostServiceBusSubscriber>();
            }
            
            var sp = services.BuildServiceProvider();
            var serviceBusPublisher = sp.GetRequiredService<IServiceBusPublisher>();
            var serviceBusSubscriber = sp.GetRequiredService<IServiceBusSubscriber>();
            
            foreach (var topic in ApplicationConstants.ServiceBus.Publish.Queues)
            {
                serviceBusPublisher.EnsureQueue(topic).GetAwaiter().GetResult();
            }
            
            foreach (var topic in ApplicationConstants.ServiceBus.Receive.Queues)
            {
                serviceBusPublisher.EnsureQueue(topic).GetAwaiter().GetResult();
            }
            
            foreach (var topic in ApplicationConstants.ServiceBus.Publish.Topics)
            {
                serviceBusPublisher.EnsureTopic(topic).GetAwaiter().GetResult();
            }
            
            foreach (var topic in ApplicationConstants.ServiceBus.Receive.Topics)
            {
                serviceBusPublisher.EnsureTopic(topic).GetAwaiter().GetResult();
            }

            foreach (var (topic, subscription) in ApplicationConstants.ServiceBus.Receive.TopicSubscriptions)
            {
                serviceBusSubscriber.EnsureTopicSubscription(topic, subscription, CancellationToken.None).GetAwaiter().GetResult();;
            }            
            
            services.AddScoped<IApplicationServiceBusClient, ApplicationServiceBusClient>();
            
            return services;
        }         
        
        private static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(InfrastructureProfile));
            services.Scan(scan =>
            {
                scan
                    .FromAssemblyOf<ITenantsRepository>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsImplementedInterfaces().WithScopedLifetime()
                    .FromAssemblyOf<TenantsRepository>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsSelf().WithScopedLifetime();
            });
        }

        private static void SetupInfrastructure(this IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var infrastucture = sp.GetService<IInfrastructure>();
            infrastucture.Setup();
        }
    }
}