using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;


namespace ApplicationBuildingBlocks.Builder.Configuration;

public static class WebApplicationBuilderConfigurationExtensions
{
	public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
	{
		builder
			.AddConfigurationFromKeyVault();
		
		return builder;
	}
	
	public static WebApplicationBuilder AddConfigurationFromKeyVault(this WebApplicationBuilder builder)
	{
		if (builder.Environment.IsProduction())
		{
			var secretClient = new SecretClient(
				new Uri($"https://{builder.Configuration["KeyVault:Name"]}.vault.azure.net/"),
				new DefaultAzureCredential());

			builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
		}

		return builder;
	}
}