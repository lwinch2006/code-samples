using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.RoleClaim;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories
{
    public interface IRoleClaimRepository
    {
        Task<IEnumerable<RoleClaimDto>> Get(Guid roleId);
        Task<RoleClaimDto> Create(CreateRoleClaimDto createRoleClaimDto);
        Task<int> Delete(DeleteRoleClaimDto deleteRoleClaimDto);        
    }
}