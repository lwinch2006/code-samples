using System;
using Microsoft.AspNetCore.Identity;

namespace Dka.Net5.IdentityWithDapper.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public override Guid Id { get; set; } = Guid.NewGuid();
    }
}