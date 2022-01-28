namespace OAuthClient.Models.Responses;

public class AuthorizationCodeRedirect : IOAuthClientResponse
{
    public string Uri { get; init; }
    public string State { get; init; }
}