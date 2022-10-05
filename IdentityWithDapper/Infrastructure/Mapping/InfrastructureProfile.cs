using AutoMapper;
using IdentityWithDapper.Infrastructure.Models.DTO.Role;
using IdentityWithDapper.Infrastructure.Models.DTO.RoleClaim;
using IdentityWithDapper.Infrastructure.Models.DTO.User;
using IdentityWithDapper.Infrastructure.Models.DTO.UserClaim;
using IdentityWithDapper.Infrastructure.Models.DTO.UserLogin;
using IdentityWithDapper.Infrastructure.Models.DTO.UserToken;
using IdentityWithDapper.Infrastructure.Models.Entities;

namespace IdentityWithDapper.Infrastructure.Mapping
{
    public class InfrastructureProfile : Profile
    {
        public InfrastructureProfile()
        {
            CreateMap<CreateUserDto, UserDto>();
            CreateMap<User, UserDto>().ReverseMap();
            
            CreateMap<CreateRoleDto, RoleDto>();
            CreateMap<Role, RoleDto>();

            CreateMap<UserToken, UserTokenDto>();
            CreateMap<CreateOrUpdateUserTokenDto, UserTokenDto>();

            CreateMap<RoleClaim, RoleClaimDto>();
            CreateMap<CreateRoleClaimDto, RoleClaimDto>();

            CreateMap<UserClaim, UserClaimDto>();
            CreateMap<CreateUserClaimDto, UserClaimDto>();

            CreateMap<UserLogin, UserLoginDto>();
            CreateMap<CreateUserLoginDto, UserLoginDto>();
        }
    }
}