using AutoMapper;
using BusinessLogic.Models.Tenants;
using Web.ViewModels.Tenants;

namespace Web.Mapping
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            CreateMap<Tenant, TenantVm>();
        }
    }
}