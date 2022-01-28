using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using OAuthClient.Models;
using OAuthClient.Models.Responses;
using OAuthClient.Utils;

namespace OAuthClient;

public interface IOAuthClient
{
    IOAuthClientResponse CreateAuthorizationCodeRequest(params string[]? scopes);
    Task<IOAuthClientResponse> ExchangeAuthorizationCodeToAccessToken(AuthorizationCodeResponse authorizationCodeResponse);
}

public class OAuthClient : IOAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly OAuthClientConfiguration _configuration;

    public OAuthClient(HttpClient httpClient, OAuthClientConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public IOAuthClientResponse CreateAuthorizationCodeRequest(params string[]? scopes)
    {
        var state = OAuthStateUtils.Generate();
            
        var queryStringParams = new Dictionary<string, string?>
        {
            {OAuthConstants.ResponseType, OAuthConstants.ResponseTypes.Code},
            {OAuthConstants.ClientId, _configuration.ClientId},
            {OAuthConstants.RedirectUri, _configuration.RedirectUri},
            {OAuthConstants.Scope, OAuthConstants.Scopes.Combine(scopes) },
            {OAuthConstants.State, state}
        };

        var redirectUri = QueryHelpers.AddQueryString(_configuration.AuthorizeEndpoint, queryStringParams);

        var response = new AuthorizationCodeRedirect
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
            {OAuthConstants.GrantType, OAuthConstants.GrantTypes.AuthorizationCode},
            {OAuthConstants.ClientId, _configuration.ClientId},
            {OAuthConstants.ClientSecret, _configuration.ClientSecret},
            {OAuthConstants.RedirectUri, _configuration.RedirectUri},
            {OAuthConstants.Code, authorizationCodeResponse.Code}
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _configuration.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestParams)
        };

        var httpResponse = await _httpClient.SendAsync(requestMessage);
        var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<AccessTokenResponse>(httpResponseContent);
        return response;
    }
}