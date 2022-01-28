using Application.HttpClients.Github;
using Application.Models.Github;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuthClient;
using OAuthClient.Models;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var githubClientOptions = new GithubClientOptions();
        configuration.Bind("GithubClientOptions", githubClientOptions);
        services.AddSingleton(githubClientOptions);

        var oauthClientConfiguration = new OAuthClientConfiguration();
        configuration.Bind("OAuthClientConfiguration", oauthClientConfiguration);
        services.AddSingleton(oauthClientConfiguration);        
        
        services.AddHttpClient<IGithubClient, GithubClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(GithubClientOptions.ApiUrlBase);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "localhost");
        });
        
        services.AddHttpClient<IOAuthClient, OAuthClient.OAuthClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(oauthClientConfiguration.BaseUri);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "localhost");
        });

        services.AddScoped<IOAuthFlows, OAuthFlows>();

        return services;
    }
}