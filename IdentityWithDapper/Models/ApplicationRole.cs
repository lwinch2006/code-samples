using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityWithDapper.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public override Guid Id { get; set; } = Guid.NewGuid();
    }
}