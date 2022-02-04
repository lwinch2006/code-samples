using OAuthClient.Models.Responses;
using OAuthClient.Utils;

namespace OAuthClient;

public interface IOAuthFlows
{
    IOAuthClientResponse RunAuthorizationCodeFlow(string[] scopes = null, string state = null);
    Task<IOAuthClientResponse> RunAuthorizationCodeFlow(AuthorizationCodeResponse authorizationCodeResponse, string originalState);
    IOAuthClientResponse RunAuthorizationCodeWithPkceFlow(string[] scopes = null, string state = null);
    Task<IOAuthClientResponse> RunAuthorizationCodeWithPkceFlow(AuthorizationCodeResponse authorizationCodeResponse, string originalState, string codeVerifier);
    IOAuthClientResponse RunImplicitFlow(string[] scopes = null, string state = null);
    IOAuthClientResponse RunImplicitFlow(ImplicitFlowResponse implicitFlowResponse, string originalState);
    Task<IOAuthClientResponse> RunPasswordFlow(string username, string password, string[] scopes = null);
    Task<IOAuthClientResponse> RunClientCredentialsFlow(string[] scopes = null);
    Task<IOAuthClientResponse> RunDeviceFlow(string[] scopes = null);
}

public class OAuthFlows : IOAuthFlows
{
    private readonly IOAuthClient _oAuthClient;

    public OAuthFlows(IOAuthClient oAuthClient)
    {
        _oAuthClient = oAuthClient;
    }

    public IOAuthClientResponse RunAuthorizationCodeFlow(string[] scopes = null, string state = null)
    {
        return _oAuthClient.CreateAuthorizationCodeRedirect(scopes, state);
    }

    public async Task<IOAuthClientResponse> RunAuthorizationCodeFlow(
        AuthorizationCodeResponse authorizationCodeResponse, 
        string originalState)
    {
        if (authorizationCodeResponse.State != originalState)
        {
            return ErrorResponseUtils.GetStateMismatchErrorResponse();
        }

        var response = await _oAuthClient.ExchangeAuthorizationCodeToAccessToken(authorizationCodeResponse);
        return response;
    }
    
    public IOAuthClientResponse RunAuthorizationCodeWithPkceFlow(string[] scopes = null, string state = null)
    {
        return _oAuthClient.CreateAuthorizationCodeWithPkceRedirect(scopes, state);
    }

    public async Task<IOAuthClientResponse> RunAuthorizationCodeWithPkceFlow(
        AuthorizationCodeResponse authorizationCodeResponse, 
        string originalState,
        string codeVerifier)
    {
        if (authorizationCodeResponse.State != originalState)
        {
            return ErrorResponseUtils.GetStateMismatchErrorResponse();
        }

        var response = await _oAuthClient.ExchangeAuthorizationCodeWithPkceToAccessToken(authorizationCodeResponse, codeVerifier);
        return response;
    }
    
    public IOAuthClientResponse RunImplicitFlow(string[] scopes = null, string state = null)
    {
        var response = _oAuthClient.CreateImplicitFlowRedirect(scopes, state);
        return response;
    }

    public IOAuthClientResponse RunImplicitFlow(
        ImplicitFlowResponse implicitFlowResponse, 
        string originalState)
    {
        if (implicitFlowResponse.State != originalState)
        {
            return ErrorResponseUtils.GetStateMismatchErrorResponse();
        }

        var accessTokenResponse = OAuthMapper.Map(implicitFlowResponse);
        return accessTokenResponse;
    }

    public async Task<IOAuthClientResponse> RunPasswordFlow(string username, string password, string[] scopes = null)
    {
        return await _oAuthClient.SendPasswordRequest(username, password, scopes);
    }

    public async Task<IOAuthClientResponse> RunClientCredentialsFlow(string[] scopes = null)
    {
        return await _oAuthClient.SendClientCredentialsRequest(scopes);
    }

    public Task<IOAuthClientResponse> RunDeviceFlow(string[] scopes = null)
    {
        return Task.FromResult((IOAuthClientResponse) null);
    }
}