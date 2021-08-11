using System;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserClaim
{
    public class CreateUserClaimDto
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public Guid UserId { get; set; }        
    }
}