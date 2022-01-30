using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using OAuthClient.Models;
using OAuthClient.Models.Constants;
using OAuthClient.Models.Responses;
using OAuthClient.Utils;

namespace OAuthClient;

public interface IOAuthClient
{
    IOAuthClientResponse CreateAuthorizationCodeRequest(string[] scopes = null, string state = null);
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
    
    public IOAuthClientResponse CreateAuthorizationCodeRequest(string[] scopes = null, string state = null)
    {
        state ??= StateUtils.Generate();
            
        var queryStringParams = new Dictionary<string, string>
        {
            {Common.ResponseType, ResponseTypes.Code},
            {Common.ClientId, _configuration.ClientId},
            {Common.RedirectUri, _configuration.RedirectUri},
            {Common.Scope, scopes?.ToString() ?? string.Empty },
            {Common.State, state}
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

        try
        {
            var httpResponse = await _httpClient.SendAsync(requestMessage);
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
            return ErrorResponseUtils.GetExchangeAuthCodeToAccessTokenErrorResponse();
        }
    }
}