using IdentityModel;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ApplicationBuildingBlocks.Builder.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static IEnumerable<string> GetNameIdentifiers(this ClaimsPrincipal principal)
    {
        var nameIdentifier = principal.Claims
            .Where(t => t.Type == ClaimTypes.NameIdentifier)
            .Select(t => t.Value);
        
        return nameIdentifier;
    }

    public static IEnumerable<string> GetPreferredUsernames(this ClaimsPrincipal principal)
    {
        var preferredUsername = principal.Claims
            .Where(t => t.Type == ClaimConstants.PreferredUserName)
            .Select(t => t.Value);
        
        return preferredUsername;
    }

    public static IEnumerable<string> GetEmailAddresses(this ClaimsPrincipal principal)
    {
        var emailAddress = principal.Claims
            .Where(t => t.Type == ClaimTypes.Email)
            .Select(t => t.Value);
        
        return emailAddress;
    }

    public static IEnumerable<string> GetIdentityProviders(this ClaimsPrincipal principal)
    {
        var identityProviders = principal.Claims
            .Where(t => t.Type is "http://schemas.microsoft.com/identity/claims/identityprovider" or JwtClaimTypes.IdentityProvider)
            .Select(t => t.Value);

        return identityProviders;
    }

    public static IEnumerable<string> GetIssuerIdentifiers(this ClaimsPrincipal principal)
    {
        var issuerIdentifiers = principal.Claims
            .Where(t => t.Type == JwtRegisteredClaimNames.Iss)
            .Select(t => t.Value);

        return issuerIdentifiers;
    }

    public static IEnumerable<string> GetIdTokens(this ClaimsPrincipal principal)
    {
        var issuerIdentifiers = principal.Claims
            .Where(t => t.Type == "id_token")
            .Select(t => t.Value);

        return issuerIdentifiers;
    }
    
    public static bool IsAzureAdMember(this ClaimsPrincipal principal)
    {
        return !principal.GetIdentityProviders().Any();
    }

    public static bool IsAzureAdGuest(this ClaimsPrincipal principal)
    {
        var claimsToCheck = principal.GetIdentityProviders();

        if (!claimsToCheck.Any())
        {
            claimsToCheck = principal.GetIssuerIdentifiers();
        }
        
        var issuerValue = principal.Claims.FirstOrDefault()?.Issuer;
        
        return claimsToCheck.Any(t => t != issuerValue);
    }	
}