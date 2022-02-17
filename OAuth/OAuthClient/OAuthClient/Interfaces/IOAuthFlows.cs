using OAuthClient.Models;
using OAuthClient.Models.Responses;

namespace OAuthClient.Interfaces;

public interface IOAuthFlows
{
    Task<IOAuthClientResponse> RunFlow(
        OAuthClientConfiguration oAuthClientConfiguration, 
        string state = null, 
        string username = null, 
        string password = null,
        string responseMode = null);

    Task<IOAuthClientResponse> RunFlow(
        OAuthClientConfiguration oAuthClientConfiguration,
        AuthorizationCodeResponse authorizationCodeResponse = null,
        ImplicitFlowResponse implicitFlowResponse = null,
        DeviceCodeResponse deviceCodeResponse = null,
        string originalState = null,
        string codeVerifier = null);
    
    IOAuthClientResponse RunAuthorizationCodeFlow(
        IEnumerable<string> scopes = null, 
        string state = null);
    
    Task<IOAuthClientResponse> RunAuthorizationCodeFlow(
        AuthorizationCodeResponse authorizationCodeResponse, 
        string originalState);
    
    IOAuthClientResponse RunAuthorizationCodeWithPkceFlow(
        IEnumerable<string> scopes = null, 
        string state = null);
    
    Task<IOAuthClientResponse> RunAuthorizationCodeWithPkceFlow(
        AuthorizationCodeResponse authorizationCodeResponse, 
        string originalState, 
        string codeVerifier);
    
    IOAuthClientResponse RunImplicitFlow(
        IEnumerable<string> scopes = null, 
        string state = null,
        string responseMode = null);
    
    IOAuthClientResponse RunImplicitFlow(
        ImplicitFlowResponse implicitFlowResponse, 
        string originalState);
    
    Task<IOAuthClientResponse> RunPasswordFlow(
        string username, 
        string password, 
        IEnumerable<string> scopes = null);
    
    Task<IOAuthClientResponse> RunClientCredentialsFlow(
        IEnumerable<string> scopes = null);
    
    Task<IOAuthClientResponse> RunDeviceFlow(
        IEnumerable<string> scopes = null);
    
    Task<IOAuthClientResponse> RunDeviceFlow(
        DeviceCodeResponse deviceCodeResponse);
}