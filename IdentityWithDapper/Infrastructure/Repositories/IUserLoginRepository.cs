using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityWithDapper.Infrastructure.Models.DTO.UserLogin;

namespace IdentityWithDapper.Infrastructure.Repositories
{
    public interface IUserLoginRepository
    {
        Task<UserLoginDto> Create(CreateUserLoginDto createUserLoginDto);
        Task<int> Delete(DeleteUserLoginDto deleteUserLoginDto);
        Task<IEnumerable<UserLoginDto>> Get(Guid userId);
    }}