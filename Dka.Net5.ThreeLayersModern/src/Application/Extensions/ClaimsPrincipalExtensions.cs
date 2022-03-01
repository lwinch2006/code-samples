using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Application.Models;
using Microsoft.Identity.Web;

namespace Application.Extensions;

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
            .Where(t => t.Type == ApplicationConstants.Authentication.Claims.IdentityProvider)
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

    public static bool IsAzureAdMember(this ClaimsPrincipal principal)
    {
        return !principal.GetIdentityProviders().Any();
    }

    public static bool IsAzureAdGuest(this ClaimsPrincipal principal)
    {
        return principal.GetIdentityProviders().Any();
    }
}