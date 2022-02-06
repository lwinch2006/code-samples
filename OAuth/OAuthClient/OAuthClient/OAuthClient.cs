using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using OAuthClient.Extensions;
using OAuthClient.Models;
using OAuthClient.Models.Constants;
using OAuthClient.Models.Responses;
using OAuthClient.Utils;

namespace OAuthClient;

public interface IOAuthClient
{
    IOAuthClientResponse CreateAuthorizationCodeRedirect(string[] scopes = null, string state = null);
    IOAuthClientResponse CreateAuthorizationCodeWithPkceRedirect(string[] scopes = null, string state = null);
    IOAuthClientResponse CreateImplicitFlowRedirect(string[] scopes = null, string state = null);
    Task<IOAuthClientResponse> ExchangeAuthorizationCodeToAccessToken(AuthorizationCodeResponse authorizationCodeResponse);
    Task<IOAuthClientResponse> ExchangeAuthorizationCodeWithPkceToAccessToken(AuthorizationCodeResponse authorizationCodeResponse, string codeVerifier);
    Task<IOAuthClientResponse> SendClientCredentialsRequest(string[] scopes = null);
    Task<IOAuthClientResponse> SendPasswordRequest(string username, string password, string[] scopes = null);
}

public class OAuthClient : IOAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly OAuthClientConfiguration _configuration;

    public OAuthClient(IHttpClientFactory httpClientFactory, OAuthClientConfiguration configuration)
    {
        var httpClient = httpClientFactory.CreateClient(nameof(OAuthClient));
        httpClient.BaseAddress = new Uri(configuration.BaseUri);
        
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public IOAuthClientResponse CreateAuthorizationCodeRedirect(string[] scopes = null, string state = null)
    {
        var queryStringParams = new Dictionary<string, string>
        {
            {Common.ResponseType, ResponseTypes.Code},
            {Common.ClientId, _configuration.ClientId},
            {Common.RedirectUri, _configuration.RedirectUri},
            {Common.State, state ?? StateUtils.Generate()}
        }.AddScopes(scopes);

        var redirectUri = QueryHelpers.AddQueryString(_configuration.AuthorizeEndpoint, queryStringParams);

        var response = new OAuthRedirect
        {
            Uri = redirectUri,
            State = state
        };

        return response;
    }

    public IOAuthClientResponse CreateAuthorizationCodeWithPkceRedirect(string[] scopes = null, string state = null)
    {
        var codeVerifier = PkceUtils.GenerateCodeVerifier();
        
        var queryStringParams = new Dictionary<string, string>
        {
            {Common.ResponseType, ResponseTypes.Code},
            {Common.ClientId, _configuration.ClientId},
            {Common.RedirectUri, _configuration.RedirectUri},
            {Common.State, state ?? StateUtils.Generate()},
            {Common.CodeChallenge, PkceUtils.GenerateCodeChallenge(codeVerifier)},
            {Common.CodeChallengeMethod, CodeChallengeMethodTypes.S256}
        }.AddScopes(scopes);
        
        var redirectUri = QueryHelpers.AddQueryString(_configuration.AuthorizeEndpoint, queryStringParams);
        
        var response = new OAuthRedirect
        {
            Uri = redirectUri,
            State = state,
            CodeVerifier = codeVerifier
        };

        return response;
    }
    
    public IOAuthClientResponse CreateImplicitFlowRedirect(string[] scopes = null, string state = null)
    {
        var queryStringParams = new Dictionary<string, string>
        {
            {Common.ResponseType, ResponseTypes.Token},
            {Common.ClientId, _configuration.ClientId},
            {Common.RedirectUri, _configuration.RedirectUri},
            {Common.State, state ?? StateUtils.Generate()}
        }.AddScopes(scopes);
        
        var redirectUri = QueryHelpers.AddQueryString(_configuration.AuthorizeEndpoint, queryStringParams);
        
        var response = new OAuthRedirect
        {
            Uri = redirectUri,
            State = state
        };

        return response;
    }

    public async Task<IOAuthClientResponse> ExchangeAuthorizationCodeToAccessToken(AuthorizationCodeResponse authorizationCodeResponse)
    {
        var requestParams = new Dictionary<string, string>
        {
            {Common.GrantType, GrantTypes.AuthorizationCode},
            {Common.ClientId, _configuration.ClientId},
            {Common.ClientSecret, _configuration.ClientSecret},
            {Common.RedirectUri, _configuration.RedirectUri},
            {Common.Code, authorizationCodeResponse.Code}
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _configuration.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestParams)
        };

        return await RunPostRequestAndReturnResult(requestMessage);
    }

    public async Task<IOAuthClientResponse> ExchangeAuthorizationCodeWithPkceToAccessToken(
        AuthorizationCodeResponse authorizationCodeResponse, string codeVerifier)
    {
        var requestParams = new Dictionary<string, string>
        {
            {Common.GrantType, GrantTypes.AuthorizationCode},
            {Common.ClientId, _configuration.ClientId},
            {Common.RedirectUri, _configuration.RedirectUri},
            {Common.Code, authorizationCodeResponse.Code},
            {Common.CodeVerifier, codeVerifier}
        }.AddClientSecret(_configuration.ClientSecret);
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _configuration.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestParams)
        };

        return await RunPostRequestAndReturnResult(requestMessage);
    }
    
    public async Task<IOAuthClientResponse> SendClientCredentialsRequest(string[] scopes = null)
    {
        var requestParams = new Dictionary<string, string>
        {
            {Common.GrantType, GrantTypes.ClientCredentials},
            {Common.ClientId, _configuration.ClientId},
            {Common.ClientSecret, _configuration.ClientSecret},
            {Common.Scope, scopes?.ToStringEx() ?? string.Empty}
        };
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _configuration.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestParams)
        };

        return await RunPostRequestAndReturnResult(requestMessage);
    }

    public async Task<IOAuthClientResponse> SendPasswordRequest(string username, string password, string[] scopes = null)
    {
        var requestParams = new Dictionary<string, string>
        {
            {Common.GrantType, GrantTypes.Password},
            {Common.ClientId, _configuration.ClientId},
            {Common.ClientSecret, _configuration.ClientSecret},
            {Common.Username, username},
            {Common.Password, password},
            {Common.Scope, scopes?.ToStringEx() ?? string.Empty}
        };
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _configuration.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestParams)
        };

        return await RunPostRequestAndReturnResult(requestMessage);
    }

    private async Task<IOAuthClientResponse> RunPostRequestAndReturnResult(HttpRequestMessage httpRequestMessage)
    {
        try
        {
            var httpResponse = await _httpClient.SendAsync(httpRequestMessage);
            var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponseContent.Contains("error"))
            {
                return JsonSerializer.Deserialize<AccessTokenResponse>(httpResponseContent);
            }

            return JsonSerializer.Deserialize<ErrorResponse>(httpResponseContent);            
        }
        catch (Exception ex)
        {
            // TODO: Logging functionality here
            return ErrorResponseUtils.GetSendClientCredentialsRequestErrorResponse();
        }
    }
}