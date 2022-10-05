using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityWithDapper.Infrastructure.Models.DTO.User;

namespace IdentityWithDapper.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto> CreateAsync(CreateUserDto createUserDto);
        Task<int> UpdateAsync(UpdateUserDto updateUserDto);
        Task<int> DeleteAsync(Guid userId);
        Task<UserDto> FindByIdAsync(Guid userId);
        Task<UserDto> FindByNameAsync(string normalizedUserName);
        Task<UserDto> FindByEmailAsync(string normalizedEmail);
        Task<UserDto> FindByLogin(string loginProvider, string providerKey);
        Task AddToRoleAsync(UserDto userDto, string roleName);
        Task RemoveFromRoleAsync(UserDto userDto, string roleName);
        Task<IList<string>> GetRolesAsync(UserDto userDto);
        Task<bool> IsInRoleAsync(UserDto userDto, string roleName);
        Task<IList<UserDto>> GetUsersInRoleAsync(string roleName);
        Task<IList<UserDto>> GetUsersForClaim(GetUsersForClaimDto getUsersForClaimDto);
        Task<IEnumerable<UserDto>> Get();
    }
}