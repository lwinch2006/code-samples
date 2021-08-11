using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.Role;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.RoleClaim;
using Dka.Net5.IdentityWithDapper.Infrastructure.Repositories;
using Dka.Net5.IdentityWithDapper.Models;
using Microsoft.AspNetCore.Identity;

namespace Dka.Net5.IdentityWithDapper.Logic
{
    public class ApplicationRoleStoreFull : 
        IQueryableRoleStore<ApplicationRole>,
        IRoleClaimStore<ApplicationRole>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleClaimRepository _roleClaimRepository;
        private readonly IMapper _mapper;

        public IQueryable<ApplicationRole> Roles => Get().GetAwaiter().GetResult();

        public ApplicationRoleStoreFull(
            IRoleRepository roleRepository,
            IRoleClaimRepository roleClaimRepository,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _roleClaimRepository = roleClaimRepository;
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

        private async Task<IQueryable<ApplicationRole>> Get(CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rolesDto = await _roleRepository.Get();

            var applicationRoles = _mapper.Map<IEnumerable<ApplicationRole>>(rolesDto);

            return applicationRoles.AsQueryable();
        }

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roleClaimsDto = await _roleClaimRepository.Get(role.Id);
            var claims = _mapper.Map<IList<Claim>>(roleClaimsDto);
            return claims;
        }

        public async Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var createRoleClaimDto = new CreateRoleClaimDto
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                RoleId = role.Id
            };

            await _roleClaimRepository.Create(createRoleClaimDto);
        }

        public async Task RemoveClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var deleteRoleClaimDto = new DeleteRoleClaimDto
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                RoleId = role.Id
            };

            await _roleClaimRepository.Delete(deleteRoleClaimDto);
        }
    }
}