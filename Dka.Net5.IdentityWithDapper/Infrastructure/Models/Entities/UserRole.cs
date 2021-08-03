using System;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Models.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }        
    }
}