using System.Security.Claims;
using Application.HttpClients.Github;
using Application.Models.Github;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using WebUI.Extensions;
using WebUI.Models.Authorization;

namespace WebUI.Controllers;

public class GithubController : Controller
{
    private readonly IGithubClient _githubClient;
    private readonly GithubClientOptions _githubClientOptions;
    
    public GithubController(IGithubClient githubClient, GithubClientOptions githubClientOptions)
    {
        _githubClient = githubClient;
        _githubClientOptions = githubClientOptions;
    }
    
    public async Task<IActionResult> ListRepositories()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return MakeAuthorizationRequest();
        }

        var accessCode = HttpContext.GetAccessCode();
        var repositories = await _githubClient.GetUserRepositoryList(accessCode);
        return View(repositories);
    }
    
    public async Task<IActionResult> Callback(string code, string state)
    {
        var accessToken = await _githubClient.GetAccessToken(code);
        
        var claims = new List<Claim>
        {
            new Claim(AuthorizationConstants.ClaimsCustomTypes.AccessToken, accessToken)
        };
                
        var claimsIdentity = new ClaimsIdentity(claims, AuthorizationConstants.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var authOptions = new AuthenticationProperties();
                
        await HttpContext.SignInAsync(AuthorizationConstants.AuthenticationScheme, claimsPrincipal,
            authOptions);

        return RedirectToAction(nameof(ListRepositories));
    }
    
    private IActionResult MakeAuthorizationRequest()
    {
        var rnd = new Random();
        var bytes = new byte[16];
        rnd.NextBytes(bytes);
        var state = BitConverter.ToString(bytes).Replace("-", string.Empty);
            
        var queryStringParams = new Dictionary<string, string?>
        {
            {"response_type", "code"},
            {"client_id", _githubClientOptions.ClientId},
            {"redirect_uri", _githubClientOptions.RedirectUrl},
            {"scope", "user public_repo"},
            {"state", state}
        };

        var githubAuthUrl = GithubClientOptions.AuthorizeUrl;
        githubAuthUrl = QueryHelpers.AddQueryString(githubAuthUrl, queryStringParams);

        return Redirect(githubAuthUrl);
    }
}