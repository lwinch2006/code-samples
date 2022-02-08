using System.Security.Claims;
using System.Text;
using Application.Models.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAuthClient;
using OAuthClient.Models;
using OAuthClient.Models.Constants;
using OAuthClient.Models.Responses;
using WebUI.Models.Authorization;
using WebUI.ViewModels.OAuth;
using OAuthMapper = WebUI.Utils.Mappers.OAuthMapper;

namespace WebUI.Controllers;

public class OAuthController : Controller
{
    private const string TestUsername = "";
    private const string TestPassword = "";
    
    private readonly OAuthClientConfiguration _oAuthClientConfiguration;
    private readonly IOAuthFlows _oAuthFlows;

    public OAuthController(IOAuthFlowsFactory oAuthFlowsFactory, IOptionsMonitor<OAuthClientConfiguration> optionsMonitor)
    {
        _oAuthClientConfiguration = optionsMonitor.Get(OAuthConfigurationNames.Github.ToLower());
        _oAuthFlows = oAuthFlowsFactory.CreateOAuthFlows(OAuthConfigurationNames.Github);
    }

    public async Task<IActionResult> Authorize(string returnUrl)
    {
        var state = Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes(returnUrl));

        var response = _oAuthClientConfiguration.FlowType switch
        {
            FlowTypes.AuthorizationCode => _oAuthFlows.RunAuthorizationCodeFlow(_oAuthClientConfiguration.Scopes, state),
            
            FlowTypes.AuthorizationCodeWithPKCE => _oAuthFlows.RunAuthorizationCodeWithPkceFlow(_oAuthClientConfiguration.Scopes, state),
            
            FlowTypes.Implicit => _oAuthFlows.RunImplicitFlow(_oAuthClientConfiguration.Scopes, state),
            
            FlowTypes.ClientCredentials => await _oAuthFlows.RunClientCredentialsFlow(_oAuthClientConfiguration.Scopes),
            
            FlowTypes.Password => await _oAuthFlows.RunPasswordFlow(TestUsername, TestPassword),
        };

        switch (response)
        {
            case OAuthRedirect oAuthRedirect:
                TempData[Common.State] = oAuthRedirect.State;
                TempData[Common.CodeVerifier] = oAuthRedirect.CodeVerifier;
                return Redirect(oAuthRedirect.Uri);                

            case AccessTokenResponse accessTokenResponse:
                return await SignInUser(accessTokenResponse, state);
            
            case ErrorResponse errorResponse:
                return ProcessOAuthClientErrorResponse(errorResponse);
        }

        return BadRequest();
    }

    public async Task<IActionResult> Callback(
        AuthorizationCodeResponseViewModel authorizationCodeResponseViewModel,
        ImplicitFlowResponseViewModel implicitFlowResponseViewModel,
        ErrorResponseViewModel errorResponseAtResponseViewModel)
    {
        if (!string.IsNullOrWhiteSpace(errorResponseAtResponseViewModel.Error))
        {
            var errorResponseAtCallback = OAuthMapper.Map(errorResponseAtResponseViewModel);
            return ProcessOAuthClientErrorResponse(errorResponseAtCallback);
        }

        var originalState = (string)TempData[Common.State];
        TempData.Remove(Common.State);

        IOAuthClientResponse response = null;

        switch (_oAuthClientConfiguration.FlowType)
        {
            case FlowTypes.AuthorizationCode:
                var authorizationCodeResponse = OAuthMapper.Map(authorizationCodeResponseViewModel);
                response = await _oAuthFlows.RunAuthorizationCodeFlow(authorizationCodeResponse, originalState);
                break;
            
            case FlowTypes.AuthorizationCodeWithPKCE:
                var codeVerifier = (string)TempData[Common.CodeVerifier];
                TempData.Remove(Common.CodeVerifier);
                authorizationCodeResponse = OAuthMapper.Map(authorizationCodeResponseViewModel);
                response = await _oAuthFlows.RunAuthorizationCodeWithPkceFlow(authorizationCodeResponse, originalState, codeVerifier);
                break;
            
            case FlowTypes.Implicit:
                var implicitFlowResponse = OAuthMapper.Map(implicitFlowResponseViewModel);
                response = _oAuthFlows.RunImplicitFlow(implicitFlowResponse, originalState);
                break;
        }

        return response switch
        {
            AccessTokenResponse accessTokenResponse => await SignInUser(accessTokenResponse, originalState),
            ErrorResponse errorResponse => ProcessOAuthClientErrorResponse(errorResponse)
        };
    }

    private async Task<IActionResult> SignInUser(AccessTokenResponse accessTokenResponse, string state)
    {
        var claims = new List<Claim>
        {
            new Claim(AuthenticationConstants.ClaimsCustomTypes.AccessToken, accessTokenResponse.AccessToken)
        };

        var claimsIdentity = new ClaimsIdentity(claims, AuthenticationConstants.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var authOptions = new AuthenticationProperties();

        await HttpContext.SignInAsync(
            AuthenticationConstants.AuthenticationScheme,
            claimsPrincipal,
            authOptions);

        var returnUrl = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(state));
        return Redirect(returnUrl);
    }

    private IActionResult ProcessOAuthClientErrorResponse(ErrorResponse errorResponse)
    {
        return Json(errorResponse);
    }
}