namespace OAuthClient.Models.Responses;

public class ImplicitFlowResponse
{
    public string AccessToken { get; init; }
    public string State { get; init; }
    public string TokenType { get; init; }
    public int ExpiresIn { get; init; }
    public string Scope { get; init; }
}