using System.Security.Claims;
using System.Text;
using Application.Models.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAuthClient;
using OAuthClient.Interfaces;
using OAuthClient.Models;
using OAuthClient.Models.Constants;
using OAuthClient.Models.Responses;
using WebUI.Extensions;
using WebUI.Models.Authorization;
using WebUI.ViewModels.OAuth;
using OAuthMapper = WebUI.Utils.Mappers.OAuthMapper;

namespace WebUI.Controllers;

public class OAuthController : Controller
{
    private readonly OAuthClientConfiguration _oAuthClientConfiguration;
    private readonly IOAuthFlows _oAuthFlows;

    public OAuthController(IOAuthFlowsFactory oAuthFlowsFactory, IOptionsMonitor<OAuthClientConfiguration> optionsMonitor)
    {
        _oAuthClientConfiguration = optionsMonitor.GetEx(OAuthConfigurationNames.Github);
        _oAuthFlows = oAuthFlowsFactory.CreateOAuthFlows(OAuthConfigurationNames.Github);
    }

    public async Task<IActionResult> Authorize(string returnUrl)
    {
        var state = Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes(returnUrl));
        var response = await _oAuthFlows.RunFlow(_oAuthClientConfiguration, state);

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

        var originalState = (string)TempData.ReadAndClear(Common.State);
        var codeVerifier = (string)TempData.ReadAndClear(Common.CodeVerifier);

        var authorizationCodeCallbackResponse = OAuthMapper.Map(authorizationCodeResponseViewModel);
        var implicitFlowCallbackResponse = OAuthMapper.Map(implicitFlowResponseViewModel);
        
        var response = await _oAuthFlows.RunFlow(
            _oAuthClientConfiguration, 
            authorizationCodeCallbackResponse, 
            implicitFlowCallbackResponse, 
            originalState: originalState, 
            codeVerifier: codeVerifier);

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