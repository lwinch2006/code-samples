using Microsoft.AspNetCore.Mvc;

namespace WebUI.ViewModels.OAuth;

public class ImplicitFlowResponseFormViewModel
{
    [FromForm(Name = "access_token")]
    public string AccessToken { get; init; }
    
    [FromForm(Name = "state")]
    public string State { get; init; }
    
    [FromForm(Name = "token_type")]
    public string TokenType { get; init; }
    
    [FromForm(Name = "expires_in")]
    public int ExpiresIn { get; init; }

    [FromForm(Name = "scope")]
    public string Scope { get; init; }

    [FromForm(Name = "session_state")]
    public string SessionState { get; init; }
}