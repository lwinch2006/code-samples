using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
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

    public OAuthController(IOAuthFlows oAuthFlows, OAuthClientConfiguration oAuthClientConfiguration)
    {
        _oAuthClientConfiguration = oAuthClientConfiguration;
        _oAuthFlows = oAuthFlows;
    }

    public async Task<IActionResult> Authorize(string returnUrl)
    {
        var state = Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes(returnUrl));

        var response = _oAuthClientConfiguration.FlowType switch
        {
            OAuthFlowTypes.AuthorizationCode => _oAuthFlows.RunAuthorizationCodeFlow(new[] { Scopes.User, Models.OAuth.Scopes.Github.PublicRepo }, state),
            
            OAuthFlowTypes.AuthorizationCodeWithPKCE => _oAuthFlows.RunAuthorizationCodeWithPkceFlow(new[] { Models.OAuth.Scopes.DemoIdentityServer.Api }, state),
            
            OAuthFlowTypes.Implicit => _oAuthFlows.RunImplicitFlow(new[] { Models.OAuth.Scopes.Curity.Email }, state),
            
            OAuthFlowTypes.ClientCredentials => await _oAuthFlows.RunClientCredentialsFlow(new[] { Models.OAuth.Scopes.DemoIdentityServer.Api }),
            
            OAuthFlowTypes.Password => await _oAuthFlows.RunPasswordFlow(TestUsername, TestPassword),
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
        ErrorCallbackViewModel errorResponseAtCallbackViewModel)
    {
        if (!string.IsNullOrWhiteSpace(errorResponseAtCallbackViewModel.Error))
        {
            var errorResponseAtCallback = OAuthMapper.Map(errorResponseAtCallbackViewModel);
            return ProcessOAuthClientErrorResponse(errorResponseAtCallback);
        }

        var originalState = (string)TempData[Common.State];
        TempData.Remove(Common.State);

        IOAuthClientResponse response = null;

        switch (_oAuthClientConfiguration.FlowType)
        {
            case OAuthFlowTypes.AuthorizationCode:
                var authorizationCodeResponse = OAuthMapper.Map(authorizationCodeResponseViewModel);
                response = await _oAuthFlows.RunAuthorizationCodeFlow(authorizationCodeResponse, originalState);
                break;
            
            case OAuthFlowTypes.AuthorizationCodeWithPKCE:
                var codeVerifier = (string)TempData[Common.CodeVerifier];
                TempData.Remove(Common.CodeVerifier);
                authorizationCodeResponse = OAuthMapper.Map(authorizationCodeResponseViewModel);
                response = await _oAuthFlows.RunAuthorizationCodeWithPkceFlow(authorizationCodeResponse, originalState, codeVerifier);
                break;
            
            case OAuthFlowTypes.Implicit:
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