using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuthClient.Interfaces;
using OAuthClient.Models;
using OAuthClient.Models.Constants;

namespace OAuthClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOAuthClient(this IServiceCollection services, IConfiguration configuration)
    {
        var oAuthClientConfigurations = configuration.GetSection(ConfigurationConstants.RootSectionName)
            .Get<List<OAuthClientConfiguration>>();

        if (!oAuthClientConfigurations.Any())
        {
            var oauthClientConfiguration = new OAuthClientConfiguration();
            configuration.Bind(ConfigurationConstants.RootSectionName, oauthClientConfiguration);
            services.AddSingleton(oauthClientConfiguration);

            services
                .AddOptions<OAuthClientConfiguration>()
                .Bind(configuration.GetSection(ConfigurationConstants.RootSectionName));
            
            services
                .AddOptions<OAuthClientConfiguration>(oauthClientConfiguration.Name.ToLower())
                .Bind(configuration.GetSection(ConfigurationConstants.RootSectionName));
        }
        else
        {
            services.AddScoped(_ => new OAuthClientConfiguration());
            
            for (var i = 0; i < oAuthClientConfigurations.Count; i++)
            {
                services
                    .AddOptions<OAuthClientConfiguration>(oAuthClientConfigurations[i].Name.ToLower())
                    .Bind(configuration.GetSection($"{ConfigurationConstants.RootSectionName}:{i}"));
            }
        }

        services.AddHttpClient(nameof(OAuthClient), httpClient =>
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "localhost");
        });

        services.AddScoped<IOAuthClient, OAuthClient>();
        services.AddScoped<IOAuthFlows, OAuthFlows>();

        services.AddScoped<IOAuthClientFactory, OAuthClientFactory>();
        services.AddScoped<IOAuthFlowsFactory, OAuthFlowsFactory>();
        
        return services;
    }
}