using System;

namespace DataAccess.DTO.Tenants
{
    public class UpdateTenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}