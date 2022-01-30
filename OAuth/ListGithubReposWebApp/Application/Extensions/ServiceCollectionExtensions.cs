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
        
        services.AddHttpClient<IGithubClient, GithubClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(githubClientOptions.ApiBaseUri);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "localhost");
        });

        return services;
    }
}