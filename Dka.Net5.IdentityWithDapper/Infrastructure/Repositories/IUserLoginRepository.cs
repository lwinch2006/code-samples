using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserLogin;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories
{
    public interface IUserLoginRepository
    {
        Task<UserLoginDto> Create(CreateUserLoginDto createUserLoginDto);
        Task<int> Delete(DeleteUserLoginDto deleteUserLoginDto);
        Task<IEnumerable<UserLoginDto>> Get(Guid userId);
    }}