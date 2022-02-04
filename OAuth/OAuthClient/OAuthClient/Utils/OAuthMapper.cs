using OAuthClient.Models.Responses;

namespace OAuthClient.Utils;

public static class OAuthMapper
{
    public static AccessTokenResponse Map(ImplicitFlowResponse source)
    {
        if (source == null)
        {
            return null;
        }

        var destination = new AccessTokenResponse
        {
            AccessToken = source.AccessToken,
            TokenType = source.TokenType,
            ExpiresIn = source.ExpiresIn,
            RefreshToken = null,
            Scope = source.Scope
        };

        return destination;
    }
}