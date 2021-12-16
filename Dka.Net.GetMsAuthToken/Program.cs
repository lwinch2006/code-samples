using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

const string tenantIdConfName = "TenantId";
const string applicationIdConfName = "ApplicationId";
const string applicationSecretConfName = "ApplicationSecret";
const string redirectUrlConfName = "RedirectUrl";

var configuration = BuildConfiguration();
var authenticationContext = new AuthenticationContext($"https://login.microsoftonline.com/{configuration[tenantIdConfName]}");
var result = await authenticationContext.AcquireTokenAsync(
    "https://management.azure.com/",
    new ClientCredential(
        configuration[applicationIdConfName], 
        configuration[applicationSecretConfName]));

if (result == null) {
    throw new InvalidOperationException("Failed to obtain the JWT token");
}

Console.WriteLine(result.AccessToken);
Console.ReadLine();

IConfiguration BuildConfiguration()
{
    var configurationBuilder = new ConfigurationBuilder()
        .AddEnvironmentVariables();

    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        ?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
    {
        configurationBuilder.AddUserSecrets<Program>();
    }
    
    return configurationBuilder.Build();
}
