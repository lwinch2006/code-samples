using AutoMapper;
using BusinessLogic.DTO.Tenants;
using BusinessLogic.Models.Tenants;

namespace BusinessLogic.Mapping
{
    public class BusinessLogicProfile : Profile
    {
        public BusinessLogicProfile()
        {
            CreateMap<DataAccess.Entities.Tenant, Tenant>();
            CreateMap<CreateTenantDto, DataAccess.DTO.Tenants.CreateTenantDto>();
            CreateMap<UpdateTenantDto, DataAccess.DTO.Tenants.UpdateTenantDto>();
            CreateMap<DeleteTenantDto, DataAccess.DTO.Tenants.DeleteTenantDto>();
        }
    }
}