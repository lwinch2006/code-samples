using System;
using System.Security.Claims;
using AutoMapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.Role;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.RoleClaim;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.User;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserClaim;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserLogin;
using Dka.Net5.IdentityWithDapper.Models;
using Microsoft.AspNetCore.Identity;

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

            CreateMap<RoleClaimDto, Claim>()
                .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src.ClaimType))
                .ForMember(dst => dst.Value, opt => opt.MapFrom(src => src.ClaimValue));
            
            CreateMap<UserClaimDto, Claim>()
                .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src.ClaimType))
                .ForMember(dst => dst.Value, opt => opt.MapFrom(src => src.ClaimValue));

            CreateMap<UserLoginDto, UserLoginInfo>();
        }
    }
}