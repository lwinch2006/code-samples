using Application.HttpClients.Github;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.Extensions;

namespace WebUI.Controllers;

[Authorize]
public class Github2Controller : Controller
{
    private readonly IGithubClient _githubClient;
    
    public Github2Controller(IGithubClient githubClient)
    {
        _githubClient = githubClient;
    }
    
    public async Task<IActionResult> ListRepositories()
    {
        var accessCode = HttpContext.GetAccessCode();
        var repositories = await _githubClient.GetUserRepositoryList(accessCode);
        return View(repositories);
    }
}