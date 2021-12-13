using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Extensions.Logging;
using ServiceBusMessages;
using ServiceBusPublisher;
using ServiceBusSubscriber;
using ThreeLayersModernEventsSubscriber.Models;
using ThreeLayersModernEventsSubscriber.Models.ServiceBus.Tenants.V1;

namespace ThreeLayersModernEventsSubscriber.Extentions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
            services.AddSingleton(new ServiceBusClient(configuration[ApplicationConstants.ServiceBus.Configuration.ConnnectionStringPath]));
            services.AddSingleton(new ServiceBusAdministrationClient(configuration[ApplicationConstants.ServiceBus.Configuration.ConnnectionStringPath]));
            services.AddSingleton(
                new ServiceBusSubscriberConfiguration
                {
                    DeserializationDictionary = new Dictionary<(int, string), Type>
                    {
                        { (1, ApplicationConstants.ServiceBus.Events.TenantEvents.TenantUpdated), typeof(ServiceBusMessage<TenantUpdatedEvent>) },
                    }
                });

            services.AddScoped<IServiceBusPublisher, ServiceBusPublisher.ServiceBusPublisher>();
            services.AddScoped<IServiceBusSubscriber, ServiceBusSubscriber.ServiceBusSubscriber>();
            services.AddScoped<IServiceBusSubscriberForDeadLetter, ServiceBusSubscriber.ServiceBusSubscriber>();

            EnsureServiceBusQueuesTopicsAndSubscriptions(configuration);
            
            return services;
    }

    public static void EnsureServiceBusQueuesTopicsAndSubscriptions(IConfiguration configuration)
    {
        var serviceBusClient =
            new ServiceBusClient(configuration[ApplicationConstants.ServiceBus.Configuration.ConnnectionStringPath]);

        var serviceBusAdmClient =
            new ServiceBusAdministrationClient(
                configuration[ApplicationConstants.ServiceBus.Configuration.ConnnectionStringPath]);

        var loggerProviders = new[] { new SerilogLoggerProvider(Log.Logger) };
        var loggerFactory = new LoggerFactory(loggerProviders);
        
        var publisherLogger = loggerFactory.CreateLogger<ServiceBusPublisher.ServiceBusPublisher>();
        var subscriberLogger = loggerFactory.CreateLogger<ServiceBusSubscriber.ServiceBusSubscriber>();

        var serviceBusPublisher = new ServiceBusPublisher.ServiceBusPublisher(serviceBusClient, serviceBusAdmClient, publisherLogger);
        var serviceBusSubscriber = new ServiceBusSubscriber.ServiceBusSubscriber(
            serviceBusClient, 
            serviceBusAdmClient, 
            new ServiceBusSubscriberConfiguration(),
            subscriberLogger);

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
            serviceBusSubscriber.EnsureTopicSubscription(topic, subscription, CancellationToken.None).GetAwaiter().GetResult();
        }
    }
    
    public static IServiceCollection AddSerilogLogger(this IServiceCollection services, IConfiguration configuration, string environmentName)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithProperty("Environment", environmentName)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .CreateLogger();
        
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger));

        return services;
    }

    public static void FixDependencies()
    {
        var files = new[]
        {
            "Microsoft.Extensions.DependencyModel.dll"
        };

        foreach (var file in files)
        {
            var sourceFile = $"bin\\ExtraDlls\\{file}";
            var targetFile = $"bin\\{file}";

            if (File.Exists(sourceFile) && !File.Exists(targetFile))
            {
                File.Copy(sourceFile, targetFile);
            }
        }
    }
}