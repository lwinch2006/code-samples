using System.Text.Json.Serialization;

namespace Application.Models.Github.Responses;

public class GithubRepositoryInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("full_name")]
    public string FullName { get; set; }
}