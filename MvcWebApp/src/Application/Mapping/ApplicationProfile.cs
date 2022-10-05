using Application.Logic.Tenants.Commands;
using Application.Models.ServiceBus.Tenants.V1;
using Application.Models.Tenants;
using Application.ViewModels.Tenants;
using AutoMapper;
using Infrastructure.DTO.Tenants;

namespace Application.Mapping
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Infrastructure.Entities.Tenant, Tenant>();
            CreateMap<Tenant, TenantVm>();
            CreateMap<CreateTenantCommand, CreateTenantDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<DeleteTenantCommand, DeleteTenantDto>();
            CreateMap<UpdateTenantCommand, UpdateTenantDto>();
            CreateMap<UpdateTenantDto, TenantUpdatedEvent>();
        }        
    }
}