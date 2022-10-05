using System;

namespace ServiceBusTester.Models.Events.V1.Tenant
{
    public class TenantUpdated
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }
    }
}