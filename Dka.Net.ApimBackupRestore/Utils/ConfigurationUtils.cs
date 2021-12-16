using Microsoft.Extensions.Configuration;

namespace Dka.Net.ApimBackupRestore.Utils;

public static class ConfigurationUtils
{
    public static IConfiguration BuildConfiguration()
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
}