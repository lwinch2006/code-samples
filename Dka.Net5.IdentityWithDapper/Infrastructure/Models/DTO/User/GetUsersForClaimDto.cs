namespace Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.User
{
    public class GetUsersForClaimDto
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }        
    }
}