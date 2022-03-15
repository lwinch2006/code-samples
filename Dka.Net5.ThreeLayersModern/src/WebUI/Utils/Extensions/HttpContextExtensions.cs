using System.Security.Claims;
using Application.Models;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Twitter;

namespace WebUI.Utils.Extensions;

public static class HttpContextExtensions
{
    public static bool IsLocalLogin(this HttpContext httpContext)
    {
        return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.HasClaim(ApplicationConstants.Authentication.Claims.Amr, "pwd");
    }

    public static bool IsMicrosoftAccountLogin(this HttpContext httpContext)
    {
        return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.HasClaim(ClaimTypes.AuthenticationMethod, MicrosoftAccountDefaults.AuthenticationScheme);
    }
        
    public static bool IsAzureAdLogin(this HttpContext httpContext)
    {
        return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.HasClaim(ClaimTypes.AuthenticationMethod, OpenIdConnectDefaults.AuthenticationScheme);
    }
        
    public static bool IsTwitterLogin(this HttpContext httpContext)
    {
        return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.HasClaim(ClaimTypes.AuthenticationMethod, TwitterDefaults.AuthenticationScheme);
    }

    public static bool IsAuthenticatedWithScheme(this HttpContext httpContext, string authenticationScheme)
    {
        return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.HasClaim(ClaimTypes.AuthenticationMethod, authenticationScheme);
    }
}