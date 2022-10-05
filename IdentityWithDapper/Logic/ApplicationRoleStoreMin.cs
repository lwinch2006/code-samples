using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IdentityWithDapper.Infrastructure.Models.DTO.Role;
using IdentityWithDapper.Infrastructure.Repositories;
using IdentityWithDapper.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityWithDapper.Logic
{
    public class ApplicationRoleStoreMin : IRoleStore<ApplicationRole>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        
        public ApplicationRoleStoreMin(
            IRoleRepository roleRepository,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }
        
        public void Dispose()
        {
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var createRoleDto = _mapper.Map<CreateRoleDto>(role);

            var roleDto = await _roleRepository.CreateAsync(createRoleDto);

            return roleDto == null
                ? IdentityResult.Failed()
                : IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var updateRoleDto = _mapper.Map<UpdateRoleDto>(role);

            var updatedRowsNum = await _roleRepository.UpdateAsync(updateRoleDto);

            return updatedRowsNum != 1
                ? IdentityResult.Failed()
                : IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var deleteRoleDto = _mapper.Map<DeleteRoleDto>(role);

            var deletedRowsNum = await _roleRepository.DeleteAsync(deleteRoleDto);

            return deletedRowsNum != 1
                ? IdentityResult.Failed()
                : IdentityResult.Success;
        }

        public async Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(role.Id.ToString());            
        }

        public async Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return await Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            role.Name = roleName;
            
            return Task.CompletedTask;
        }

        public async Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return await Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Guid.TryParse(roleId, out var roleIdAsGuid))
            {
                return null;
            }

            var roleDto = await _roleRepository.FindByIdAsync(roleIdAsGuid);
            var applicationRole = _mapper.Map<ApplicationRole>(roleDto);
            return applicationRole;
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roleDto = await _roleRepository.FindByNameAsync(normalizedRoleName);
            var applicationRole = _mapper.Map<ApplicationRole>(roleDto);
            return applicationRole;
        }
    }
}