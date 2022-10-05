using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityWithDapper.Infrastructure.Models.DTO.RoleClaim;

namespace IdentityWithDapper.Infrastructure.Repositories
{
    public interface IRoleClaimRepository
    {
        Task<IEnumerable<RoleClaimDto>> Get(Guid roleId);
        Task<RoleClaimDto> Create(CreateRoleClaimDto createRoleClaimDto);
        Task<int> Delete(DeleteRoleClaimDto deleteRoleClaimDto);        
    }
}