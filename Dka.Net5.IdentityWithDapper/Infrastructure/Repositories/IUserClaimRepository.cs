using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserClaim;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories
{
    public interface IUserClaimRepository
    {
        Task<IEnumerable<UserClaimDto>> Get(Guid userId);
        Task<int> Create(IEnumerable<CreateUserClaimDto> createUserClaimsDto);
        Task<int> Update(UpdateUserClaimDto updateUserClaimDto);
        Task<int> Delete(IEnumerable<DeleteUserClaimDto> deleteUserClaimsDto);
    }
}