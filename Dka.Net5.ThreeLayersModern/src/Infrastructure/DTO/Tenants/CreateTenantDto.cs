using System;

namespace Infrastructure.DTO.Tenants
{
    public class CreateTenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}