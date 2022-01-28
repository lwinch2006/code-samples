using Application.Models.Github.Responses;

namespace Application.HttpClients.Github;

public interface IGithubClient
{
    Task<IEnumerable<GithubRepositoryInfo>> GetUserRepositoryList(string accessCode);
    Task<string> GetAccessToken(string code);
}