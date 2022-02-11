using WebUI.Models.Authorization;

namespace WebUI.Extensions;

public static class HttpContextExtensions
{
    public static string GetAccessCode(this HttpContext httpContext)
    {
        var accessCode =
            httpContext.User.Claims.Single(_ =>
                _.Type == AuthenticationConstants.ClaimsCustomTypes.AccessToken);
        
        return accessCode.Value;
    }
}