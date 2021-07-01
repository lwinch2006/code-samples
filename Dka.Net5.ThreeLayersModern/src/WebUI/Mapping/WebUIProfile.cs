using Application.Logic.Tenants.Commands;
using Application.ViewModels.Tenants;
using AutoMapper;

namespace WebUI.Mapping
{
    public class WebUIProfile : Profile
    {
        public WebUIProfile()
        {
            CreateMap<TenantVm, UpdateTenantCommand>();
        }
    }
}