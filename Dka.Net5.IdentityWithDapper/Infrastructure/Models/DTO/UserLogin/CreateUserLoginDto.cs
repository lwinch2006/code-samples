using System;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserLogin
{
    public class CreateUserLoginDto
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public Guid UserId { get; set; }        
    }
}