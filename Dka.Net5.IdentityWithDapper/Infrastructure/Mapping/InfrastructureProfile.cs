using AutoMapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.Role;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.RoleClaim;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.User;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserClaim;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserLogin;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserToken;
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