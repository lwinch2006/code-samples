using System;

namespace IdentityWithDapper.Infrastructure.Models.DTO.UserClaim
{
    public class DeleteUserClaimDto
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public Guid UserId { get; set; }           
    }
}