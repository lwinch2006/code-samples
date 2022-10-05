namespace IdentityWithDapper.Infrastructure.Models.Entities
{
    public class RoleClaim
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public int RoleId { get; set; }
    }
}