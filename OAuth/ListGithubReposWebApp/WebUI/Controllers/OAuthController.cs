using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OAuthClient;
using OAuthClient.Models;
using OAuthClient.Models.Responses;
using WebUI.Models.Authorization;

namespace WebUI.Controllers;

public class OAuthController : Controller
{
    private readonly IOAuthFlows _oAuthFlows;
    
    public OAuthController(IOAuthFlows oAuthFlows)
    {
        _oAuthFlows = oAuthFlows;
    }

    public IActionResult Authorize()
    {
        return Redirect(((AuthorizationCodeRedirect)_oAuthFlows.RunAuthorizationCodeFlow(new[] { OAuthConstants.Scopes.User, OAuthConstants.Scopes.Github.PublicRepo })).Uri);
    }
    
    public async Task<IActionResult> Callback(AuthorizationCodeResponse authorizationCodeResponse)
    {
        var response = await _oAuthFlows.RunAuthorizationCodeFlow(authorizationCodeResponse, authorizationCodeResponse.State);

        return response switch
        {
            AccessTokenResponse accessTokenResponse => await SignInUser(accessTokenResponse),
            OAuthClientErrorResponse errorResponse => await ProcessOAuthClientErrorResponse(errorResponse)
        };
    }

    // public async Task<IActionResult> Callback(string error, string errorDescription, string errorUri)
    // {
    //     var oAuthClientErrorResponse = new OAuthClientErrorResponse
    //     {
    //         Error = error,
    //         ErrorDescription = errorDescription,
    //         ErrorUri = errorUri
    //     };
    //     
    //     return await ProcessOAuthClientErrorResponse(oAuthClientErrorResponse);
    // }

    private async Task<IActionResult> SignInUser(AccessTokenResponse accessTokenResponse)
    {
        var claims = new List<Claim>
        {
            new Claim(AuthorizationConstants.ClaimsCustomTypes.AccessToken, accessTokenResponse.AccessToken)
        };
                
        var claimsIdentity = new ClaimsIdentity(claims, AuthorizationConstants.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var authOptions = new AuthenticationProperties();
                
        await HttpContext.SignInAsync(
            AuthorizationConstants.AuthenticationScheme, 
            claimsPrincipal,
            authOptions);

        return RedirectToAction("Index", "Home");
    }

    private async Task<IActionResult> ProcessOAuthClientErrorResponse(OAuthClientErrorResponse oAuthClientErrorResponse)
    {
        return await Task.FromResult(Ok());
    }
}