using System.Text.Json;
using Application.Models.Github.Responses;

namespace Application.HttpClients.Github;

public class GithubClient : IGithubClient
{
    private readonly HttpClient _httpClient;

    public GithubClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
}