using System;
using System.Security.Claims;
using AutoMapper;
using IdentityWithDapper.Infrastructure.Models.DTO.UserLogin;
using IdentityWithDapper.Infrastructure.Models.DTO.Role;
using IdentityWithDapper.Infrastructure.Models.DTO.RoleClaim;
using IdentityWithDapper.Infrastructure.Models.DTO.User;
using IdentityWithDapper.Infrastructure.Models.DTO.UserClaim;
using IdentityWithDapper.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityWithDapper.Mapping
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

            CreateMap<RoleClaimDto, Claim>()
                .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src.ClaimType))
                .ForMember(dst => dst.Value, opt => opt.MapFrom(src => src.ClaimValue));
            
            CreateMap<UserClaimDto, Claim>()
                .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src.ClaimType))
                .ForMember(dst => dst.Value, opt => opt.MapFrom(src => src.ClaimValue));
        }
    }
}