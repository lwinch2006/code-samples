namespace OAuthClient.Models.Responses;

public class OAuthRedirect : IOAuthClientResponse
{
    public string Uri { get; init; }
    public string State { get; init; }
    public string CodeVerifier { get; init; }
}