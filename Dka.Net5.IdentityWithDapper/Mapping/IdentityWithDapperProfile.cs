using System;
using AutoMapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.Role;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.User;
using Dka.Net5.IdentityWithDapper.Models;

namespace Dka.Net5.IdentityWithDapper.Mapping
{
    public class IdentityWithDapperProfile : Profile
    {
        public IdentityWithDapperProfile()
        {
            CreateMap<ApplicationUser, CreateUserDto>();
            CreateMap<ApplicationUser, UpdateUserDto>();
            CreateMap<ApplicationUser, DeleteUserDto>();
            CreateMap<CreateUserDto, UserDto>();
            CreateMap<UserDto, ApplicationUser>().ReverseMap();
            
            CreateMap<ApplicationRole, CreateRoleDto>();
            CreateMap<ApplicationRole, UpdateRoleDto>();
            CreateMap<ApplicationRole, DeleteRoleDto>();
            CreateMap<RoleDto, ApplicationRole>();
        }
    }
}