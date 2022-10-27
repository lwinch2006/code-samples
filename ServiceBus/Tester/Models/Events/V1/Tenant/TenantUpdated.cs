using System;

namespace ServiceBusTester.Models.Events.V1.Tenant
{
    public class TenantUpdated
    {
        public Guid Id { get; init; }
        public string NewName { get; init; } = default!;
    }
}