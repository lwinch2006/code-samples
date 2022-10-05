using System;

namespace IdentityWithDapper.Infrastructure.Models.DTO.RoleClaim
{
    public class CreateRoleClaimDto
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public Guid RoleId { get; set; }
    }
}