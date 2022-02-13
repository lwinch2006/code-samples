using OAuthClient.Models.Responses;

namespace OAuthClient.Interfaces;

public interface IOAuthClient
{
    IOAuthClientResponse CreateAuthorizationCodeRedirect(IEnumerable<string> scopes = null, string state = null);
    IOAuthClientResponse CreateAuthorizationCodeWithPkceRedirect(IEnumerable<string> scopes = null, string state = null);
    IOAuthClientResponse CreateImplicitFlowRedirect(IEnumerable<string> scopes = null, string state = null);
    Task<IOAuthClientResponse> ExchangeAuthorizationCodeToAccessToken(AuthorizationCodeResponse authorizationCodeResponse);
    Task<IOAuthClientResponse> ExchangeAuthorizationCodeWithPkceToAccessToken(AuthorizationCodeResponse authorizationCodeResponse, string codeVerifier);
    Task<IOAuthClientResponse> SendClientCredentialsRequest(IEnumerable<string> scopes = null);
    Task<IOAuthClientResponse> SendPasswordRequest(string username, string password, IEnumerable<string> scopes = null);
    Task<IOAuthClientResponse> SendDeviceCodeRequest(IEnumerable<string> scopes = null);
    Task<IOAuthClientResponse> SendDeviceTokenRequest(DeviceCodeResponse deviceCodeResponse);
}