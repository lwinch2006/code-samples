using AutoMapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.Role;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.User;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.Entities;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Mapping
{
    public class InfrastructureProfile : Profile
    {
        public InfrastructureProfile()
        {
            CreateMap<CreateUserDto, UserDto>();
            CreateMap<User, UserDto>().ReverseMap();
            
            CreateMap<CreateRoleDto, RoleDto>();
            CreateMap<Role, RoleDto>();
        }
    }
}