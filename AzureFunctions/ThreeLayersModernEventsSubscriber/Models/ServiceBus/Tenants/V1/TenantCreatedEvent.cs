using System;

namespace ThreeLayersModernEventsSubscriber.Models.ServiceBus.Tenants.V1;

public class TenantCreatedEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}