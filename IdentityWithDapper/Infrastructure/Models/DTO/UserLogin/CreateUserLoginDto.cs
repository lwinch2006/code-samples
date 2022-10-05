using System;

namespace IdentityWithDapper.Infrastructure.Models.DTO.UserLogin
{
    public class CreateUserLoginDto
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public Guid UserId { get; set; }        
    }
}