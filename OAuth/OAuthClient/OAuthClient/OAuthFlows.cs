using OAuthClient.Models.Responses;
using OAuthClient.Utils;

namespace OAuthClient;

public interface IOAuthFlows
{
    IOAuthClientResponse RunAuthorizationCodeFlow(string[] scopes = null, string state = null);
    Task<IOAuthClientResponse> RunAuthorizationCodeFlow(AuthorizationCodeResponse authorizationCodeResponse, string originalState);
    Task<IOAuthClientResponse> RunAuthorizationFlowWithPKCE();
    Task<IOAuthClientResponse> RunImplicitFlow();
    Task<IOAuthClientResponse> RunPasswordFlow();
    Task<IOAuthClientResponse> RunClientCredentialsFlow();
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
        var response = _oAuthClient.CreateAuthorizationCodeRequest(scopes, state);
        return response;
    }

    public async Task<IOAuthClientResponse> RunAuthorizationCodeFlow(AuthorizationCodeResponse authorizationCodeResponse, string originalState)
    {
        if (authorizationCodeResponse.State != originalState)
        {
            return ErrorResponseUtils.GetStateMismatchErrorResponse();
        }

        var response = await _oAuthClient.ExchangeAuthorizationCodeToAccessToken(authorizationCodeResponse);
        return response;
    }
    
    public Task<IOAuthClientResponse> RunAuthorizationFlowWithPKCE()
    {
        throw new NotImplementedException();
    }

    public Task<IOAuthClientResponse> RunImplicitFlow()
    {
        throw new NotImplementedException();
    }

    public Task<IOAuthClientResponse> RunPasswordFlow()
    {
        throw new NotImplementedException();
    }

    public Task<IOAuthClientResponse> RunClientCredentialsFlow()
    {
        throw new NotImplementedException();
    }
}