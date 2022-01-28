namespace OAuthClient.Models;

public static class OAuthConstants
{
    public const string ResponseType = "response_type";
    public const string ClientId = "client_id";
    public const string ClientSecret = "client_secret";
    public const string RedirectUri = "redirect_uri";
    public const string Scope = "scope";
    public const string State = "state";
    public const string Code = "code";
    public const string GrantType = "grant_type";

    public static class ResponseTypes
    {
        public const string Code = "code";
    }

    public static class Scopes
    {
        public const string User = "user";

        public class Github
        {
            public const string PublicRepo = "public_repo";
        }

        public static string Combine(string[]? scopes = null)
        {
            return string.Join(' ', scopes ?? Array.Empty<string>());
        }
    }

    public static class GrantTypes
    {
        public const string AuthorizationCode = "authorization_code";
    }
    

}