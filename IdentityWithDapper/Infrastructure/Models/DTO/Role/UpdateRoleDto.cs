﻿using System;

namespace IdentityWithDapper.Infrastructure.Models.DTO.Role
{
    public class UpdateRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; } 
    }
}