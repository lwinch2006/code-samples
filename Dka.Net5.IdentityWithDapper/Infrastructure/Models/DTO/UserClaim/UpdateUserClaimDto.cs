using System;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserClaim
{
    public class UpdateUserClaimDto
    {
        public string OldClaimType { get; set; }
        public string OldClaimValue { get; set; }

        public string NewClaimType { get; set; }
        public string NewClaimValue { get; set; }

        public Guid UserId { get; set; }           
    }
}