using OAuthClient.Models.Constants;

namespace OAuthClient.Extensions;

public static class QueryStringExtensions
{
    public static Dictionary<string, string> AddScopes(
        this Dictionary<string, string> queryStringParams,
        IEnumerable<string> scopes)
    {
        var scopesAsString = scopes?.ToStringEx();

        if (!string.IsNullOrWhiteSpace(scopesAsString))
        {
            queryStringParams.Add(Common.Scope, scopesAsString);
        }

        return queryStringParams;
    }

    public static Dictionary<string, string> AddResponseMode(
        this Dictionary<string, string> queryStringParams,
        string responseMode)
    {
        if (string.IsNullOrWhiteSpace(responseMode))
        {
            return queryStringParams;
        }
        
        queryStringParams.Add(Common.ResponseMode, responseMode);

        return queryStringParams;
    }

    public static Dictionary<string, string> AddClientSecret(this Dictionary<string, string> queryStringParams, string clientSecret)
    {
        if (!string.IsNullOrWhiteSpace(clientSecret))
        {
            queryStringParams.Add(Common.ClientSecret, clientSecret);
        }

        return queryStringParams;
    }
}