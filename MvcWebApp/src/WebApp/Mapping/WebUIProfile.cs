using Application.ViewModels.Tenants;
using AutoMapper;

namespace WebApp.Mapping
{
	public class WebUIProfile : Profile
	{
		public WebUIProfile()
		{
			CreateMap<TenantVm, UpdateTenantCommand>();
		}
	}
}