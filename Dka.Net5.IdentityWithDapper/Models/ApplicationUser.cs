using System;
using Microsoft.AspNetCore.Identity;

namespace Dka.Net5.IdentityWithDapper.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public override Guid Id { get; set; } = Guid.NewGuid();
    }
}