using Application.HttpClients.Github;
using Application.Models.Github;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IGithubClient, GithubClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(GithubClientOptions.ApiUrlBase);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "localhost");
        });

        var githubClientOptions = new GithubClientOptions();
        configuration.Bind("GithubClientOptions", githubClientOptions);
        services.AddSingleton(githubClientOptions);

        return services;
    }
}