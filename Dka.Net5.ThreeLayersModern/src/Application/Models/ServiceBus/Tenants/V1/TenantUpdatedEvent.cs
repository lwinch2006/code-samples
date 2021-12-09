namespace Application.Models.ServiceBus.Tenants.V1;

public class TenantUpdatedEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}