using Application.Models.Tenants;
using Application.ViewModels.Tenants;
using AutoMapper;

namespace Application.Mapping
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Infrastructure.Entities.Tenant, Tenant>();
            CreateMap<Tenant, TenantVm>();
        }        
    }
}