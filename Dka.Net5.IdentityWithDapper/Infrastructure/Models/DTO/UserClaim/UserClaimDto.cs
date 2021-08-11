namespace Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserClaim
{
    public class UserClaimDto
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public int UserId { get; set; }         
    }
}