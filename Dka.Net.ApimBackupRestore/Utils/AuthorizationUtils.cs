using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Dka.Net.ApimBackupRestore.Utils;

public static class AuthorizationUtils
{
    private const string TenantIdConfName = "TenantId";
    private const string ApplicationIdConfName = "ApplicationId";
    private const string ApplicationSecretConfName = "ApplicationSecret";
    private const string RedirectUrlConfName = "RedirectUrl";

    public static async Task<string> GetBearerToken(IConfiguration configuration)
    {
        var authenticationContext = new AuthenticationContext($"https://login.microsoftonline.com/{configuration[TenantIdConfName]}");
        var result = await authenticationContext.AcquireTokenAsync(
            "https://management.azure.com/",
            new ClientCredential(
                configuration[ApplicationIdConfName], 
                configuration[ApplicationSecretConfName]));
        
        if (result == null) {
            throw new InvalidOperationException("Failed to obtain the JWT token");
        }

        return result.AccessToken;
    }
}