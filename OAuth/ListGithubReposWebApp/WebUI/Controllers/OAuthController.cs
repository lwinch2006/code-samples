using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OAuthClient;
using OAuthClient.Models.Constants;
using OAuthClient.Models.Responses;
using WebUI.Models.Authorization;
using WebUI.Utils.Mappers;
using WebUI.ViewModels.OAuth;

namespace WebUI.Controllers;

public class OAuthController : Controller
{
    private readonly IOAuthFlows _oAuthFlows;
    
    public OAuthController(IOAuthFlows oAuthFlows)
    {
        _oAuthFlows = oAuthFlows;
    }

    public IActionResult Authorize(string returnUrl)
    {
        var state = Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes(returnUrl));

        TempData[Common.State] = state;
        
        return Redirect(((AuthorizationCodeRedirect)_oAuthFlows.RunAuthorizationCodeFlow(
            new[]
            {
                Scopes.User, 
                GithubScopes.PublicRepo
            },
            state
            )).Uri);
    }
    
    public async Task<IActionResult> Callback(AuthorizationCodeResponseViewModel authorizationCodeResponseViewModel, ErrorCallbackViewModel errorResponseAtCallbackViewModel)
    {
        var errorResponseAtCallback = OAuthMapper.Map(errorResponseAtCallbackViewModel);
        
        if (!string.IsNullOrWhiteSpace(errorResponseAtCallbackViewModel.Error))
        {
            return await ProcessOAuthClientErrorResponse(errorResponseAtCallback);
        }

        var originalState = (string)TempData[Common.State];
        TempData.Remove(Common.State);
        
        var authorizationCodeResponse = OAuthMapper.Map(authorizationCodeResponseViewModel);
        var response = await _oAuthFlows.RunAuthorizationCodeFlow(authorizationCodeResponse, originalState);

        return response switch
        {
            AccessTokenResponse accessTokenResponse => await SignInUser(accessTokenResponse, authorizationCodeResponse.State),
            ErrorResponse errorResponse => await ProcessOAuthClientErrorResponse(errorResponse)
        };
    }

    private async Task<IActionResult> SignInUser(AccessTokenResponse accessTokenResponse, string state)
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

        var returnUrl = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(state));
        return Redirect(returnUrl);
    }
    
    private async Task<IActionResult> ProcessOAuthClientErrorResponse(ErrorResponse errorResponse)
    {
        return await Task.FromResult(Json(errorResponse));
    }
}