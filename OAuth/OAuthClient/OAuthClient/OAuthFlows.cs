﻿using OAuthClient.Interfaces;
using OAuthClient.Models;
using OAuthClient.Models.Constants;
using OAuthClient.Models.Responses;
using OAuthClient.Utils;

namespace OAuthClient;

public class OAuthFlows : IOAuthFlows
{
    private readonly IOAuthClient _oAuthClient;

    public OAuthFlows(IOAuthClient oAuthClient)
    {
        _oAuthClient = oAuthClient;
    }

    public async Task<IOAuthClientResponse> RunFlow(
        OAuthClientConfiguration oAuthClientConfiguration, 
        string state = null, 
        string username = null, 
        string password = null,
        string responseMode = null)
    {
        var response = oAuthClientConfiguration.FlowType switch
        {
            FlowTypes.AuthorizationCode => RunAuthorizationCodeFlow(oAuthClientConfiguration.Scopes, state),
            
            FlowTypes.AuthorizationCodeWithPKCE => RunAuthorizationCodeWithPkceFlow(oAuthClientConfiguration.Scopes, state),
            
            FlowTypes.Implicit => RunImplicitFlow(oAuthClientConfiguration.Scopes, state, responseMode),
            
            FlowTypes.ClientCredentials => await RunClientCredentialsFlow(oAuthClientConfiguration.Scopes),
            
            FlowTypes.Password => await RunPasswordFlow(username, password, oAuthClientConfiguration.Scopes),
            
            FlowTypes.Device => await RunDeviceFlow(oAuthClientConfiguration.Scopes)
        };

        return response;
    }

    public async Task<IOAuthClientResponse> RunFlow(
        OAuthClientConfiguration oAuthClientConfiguration,
        AuthorizationCodeResponse authorizationCodeResponse = null,
        ImplicitFlowResponse implicitFlowResponse = null,
        DeviceCodeResponse deviceCodeResponse = null,
        string originalState = null,
        string codeVerifier = null)
    {
        return oAuthClientConfiguration.FlowType switch
        {
            FlowTypes.AuthorizationCode => await RunAuthorizationCodeFlow(authorizationCodeResponse, originalState),
            FlowTypes.AuthorizationCodeWithPKCE => await RunAuthorizationCodeWithPkceFlow(authorizationCodeResponse,originalState, codeVerifier),
            FlowTypes.Implicit => RunImplicitFlow(implicitFlowResponse, originalState),
            FlowTypes.Device => await RunDeviceFlow(deviceCodeResponse),
            _ => null
        };
    }
    
    public IOAuthClientResponse RunAuthorizationCodeFlow(
        IEnumerable<string> scopes = null, 
        string state = null)
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
    
    public IOAuthClientResponse RunAuthorizationCodeWithPkceFlow(
        IEnumerable<string> scopes = null, 
        string state = null)
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
    
    public IOAuthClientResponse RunImplicitFlow(
        IEnumerable<string> scopes = null, 
        string state = null,
        string responseMode = null)
    {
        var response = _oAuthClient.CreateImplicitFlowRedirect(scopes, state, responseMode);
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

    public async Task<IOAuthClientResponse> RunPasswordFlow(
        string username, 
        string password, 
        IEnumerable<string> scopes = null)
    {
        return await _oAuthClient.SendPasswordRequest(username, password, scopes);
    }

    public async Task<IOAuthClientResponse> RunClientCredentialsFlow(
        IEnumerable<string> scopes = null)
    {
        return await _oAuthClient.SendClientCredentialsRequest(scopes);
    }

    public async Task<IOAuthClientResponse> RunDeviceFlow(
        IEnumerable<string> scopes = null)
    {
        return await _oAuthClient.SendDeviceCodeRequest(scopes);
    }
    
    public async Task<IOAuthClientResponse> RunDeviceFlow(
        DeviceCodeResponse deviceCodeResponse)
    {
        return await _oAuthClient.SendDeviceTokenRequest(deviceCodeResponse);
    }    
}