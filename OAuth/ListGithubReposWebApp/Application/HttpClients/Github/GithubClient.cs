using System.Text.Json;
using Application.Models.Github;
using Application.Models.Github.Responses;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.HttpClients.Github;

public class GithubClient : IGithubClient
{
    private readonly HttpClient _httpClient;
    private readonly GithubClientOptions _githubClientOptions;
    
    public GithubClient(HttpClient httpClient, GithubClientOptions githubClientOptions)
    {
        _httpClient = httpClient;
        _githubClientOptions = githubClientOptions;
    }

    public async Task<IEnumerable<GithubRepositoryInfo>> GetUserRepositoryList(string accessCode)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/user/repos?sort=name&direction=desc");
        requestMessage.Headers.Add("Authorization", $"Bearer {accessCode}");

        var response = await _httpClient.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var userRepositories = JsonSerializer.Deserialize<IEnumerable<GithubRepositoryInfo>>(responseContent);
        
        return userRepositories ?? Enumerable.Empty<GithubRepositoryInfo>();
    }

    public async Task<string> GetAccessToken(string code)
    {
        var requestParams = new Dictionary<string, string>
        {
            {"grant_type", "authorization_code"},
            {"client_id", _githubClientOptions.ClientId},
            {"client_secret", _githubClientOptions.ClientSecret},
            {"redirect_uri", _githubClientOptions.RedirectUrl},
            {"code", code}
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, GithubClientOptions.TokenUrl)
        {
            Content = new FormUrlEncodedContent(requestParams)
        };

        var response = await _httpClient.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseValues = JsonSerializer.Deserialize<IDictionary<string,string>>(responseContent);
        return responseValues!["access_token"];
    }
}