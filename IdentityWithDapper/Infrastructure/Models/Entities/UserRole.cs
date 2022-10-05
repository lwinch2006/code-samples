using System;

namespace IdentityWithDapper.Infrastructure.Models.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }        
    }
}