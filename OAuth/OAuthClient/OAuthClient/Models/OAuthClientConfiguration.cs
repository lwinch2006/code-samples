namespace OAuthClient.Models;

public class OAuthClientConfiguration
{
    public string BaseUri { get; init; }
    public string AuthorizeEndpoint { get; init; }
    public string TokenEndpoint { get; init; }
    public string RedirectUri { get; init; }
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
}