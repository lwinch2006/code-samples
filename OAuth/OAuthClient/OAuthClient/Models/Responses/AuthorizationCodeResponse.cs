namespace OAuthClient.Models.Responses;

public class AuthorizationCodeResponse : IOAuthClientResponse
{
    public string Code { get; init; }
    public string State { get; init; }
}