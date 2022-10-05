namespace IdentityWithDapper.Infrastructure.Models.Entities
{
    public class UserClaim
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public int UserId { get; set; }        
    }
}