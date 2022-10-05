using System.Threading;
using Application.Mapping;
using Application.Models;
using Application.Models.ServiceBus.Tenants.V1;
using Microsoft.Extensions.Logging;
using ServiceBusPublisher;
using ServiceBusSubscriber;

namespace Application.Logic.ServiceBus;

public interface IApplicationServiceBusClient
{
    Task SendTenantUpdatedEvent(TenantUpdatedEvent payload, string createdBy, CancellationToken token);
}

public class ApplicationServiceBusClient : IApplicationServiceBusClient
{
    private readonly IServiceBusPublisher _serviceBusPublisher;
    private readonly IServiceBusSubscriber _serviceBusSubscriber;
    private readonly IServiceBusSubscriberForDeadLetter _serviceBusSubscriberForDeadLetter;
    private readonly ILogger<ApplicationServiceBusClient> _logger;

    public ApplicationServiceBusClient(
        IServiceBusPublisher serviceBusPublisher,
        IServiceBusSubscriber serviceBusSubscriber,
        IServiceBusSubscriberForDeadLetter serviceBusSubscriberForDeadLetter,
        ILogger<ApplicationServiceBusClient> logger)
    {
        _serviceBusPublisher = serviceBusPublisher;
        _serviceBusSubscriber = serviceBusSubscriber;
        _serviceBusSubscriberForDeadLetter = serviceBusSubscriberForDeadLetter;
        _logger = logger;
    }
    
    public async Task SendTenantUpdatedEvent(TenantUpdatedEvent payload, string createdBy, CancellationToken token)
    {
        var message = ServiceBusEventMapper.Map(
            nameof(ApplicationConstants.ServiceBus.Events.TenantEvents),
            nameof(ApplicationConstants.ServiceBus.Events.TenantEvents.TenantUpdated),
            1,
            createdBy,
            payload);

        await _serviceBusPublisher.SendMessage(ApplicationConstants.ServiceBus.Publish.SendEventsTopic, message, token);
    }
}