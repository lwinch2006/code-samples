using System.Security.Claims;
using Application.Models;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Http;

namespace WebUI.Utils.Extensions
{
    public static class ServiceCollectionExtensions
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
    }
}