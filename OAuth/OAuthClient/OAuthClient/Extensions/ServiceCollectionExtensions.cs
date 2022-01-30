using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuthClient.Models;

namespace OAuthClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOAuthClient(this IServiceCollection services, IConfiguration configuration)
    {
        var oauthClientConfiguration = new OAuthClientConfiguration();
        configuration.Bind("OAuthClientConfiguration", oauthClientConfiguration);
        services.AddSingleton(oauthClientConfiguration);
        
        services.AddHttpClient<IOAuthClient, OAuthClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(oauthClientConfiguration.BaseUri);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "localhost");
        });

        services.AddScoped<IOAuthFlows, OAuthFlows>();

        return services;
    }
}