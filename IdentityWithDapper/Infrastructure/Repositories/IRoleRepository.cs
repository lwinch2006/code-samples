using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityWithDapper.Infrastructure.Models.DTO.Role;

namespace IdentityWithDapper.Infrastructure.Repositories
{
    public interface IRoleRepository
    {
        Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto);
        Task<int> UpdateAsync(UpdateRoleDto updateRoleDto);
        Task<int> DeleteAsync(DeleteRoleDto deleteRoleDto);
        Task<RoleDto> FindByIdAsync(Guid roleId);
        Task<RoleDto> FindByNameAsync(string normalizedRoleName);
        Task<IEnumerable<RoleDto>> Get();
    }}