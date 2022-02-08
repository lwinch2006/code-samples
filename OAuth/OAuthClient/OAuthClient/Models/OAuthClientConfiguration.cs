using OAuthClient.Models.Constants;

namespace OAuthClient.Models;

public class OAuthClientConfiguration
{
    public string Name { get; init; }
    public string BaseUri { get; init; }
    public string AuthorizeEndpoint { get; init; }
    public string TokenEndpoint { get; init; }
    public string RedirectUri { get; init; }
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public FlowTypes FlowType { get; init; }
    public IEnumerable<string> Scopes { get; init; }
}