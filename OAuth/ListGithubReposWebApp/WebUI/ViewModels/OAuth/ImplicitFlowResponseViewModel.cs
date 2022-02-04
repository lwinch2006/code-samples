using Microsoft.AspNetCore.Mvc;

namespace WebUI.ViewModels.OAuth;

public class ImplicitFlowResponseViewModel
{
    [FromQuery(Name = "access_token")]
    public string AccessToken { get; init; }
    
    [FromQuery(Name = "state")]
    public string State { get; init; }
    
    [FromQuery(Name = "token_type")]
    public string TokenType { get; init; }
    
    [FromQuery(Name = "expires_in")]
    public int ExpiresIn { get; init; }

    [FromQuery(Name = "scope")]
    public string Scope { get; init; }
}