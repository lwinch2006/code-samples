﻿using System;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.RoleClaim
{
    public class DeleteRoleClaimDto
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public Guid RoleId { get; set; }        
    }
}