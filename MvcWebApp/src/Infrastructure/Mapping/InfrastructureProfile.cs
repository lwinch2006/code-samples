using AutoMapper;
using Infrastructure.DTO.Tenants;
using Infrastructure.Entities;

namespace Infrastructure.Mapping
{
    public class InfrastructureProfile : Profile
    {
        public InfrastructureProfile()
        {
            CreateMap<CreateTenantDto, Tenant>();
        }
    }
}