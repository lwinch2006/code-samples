using Microsoft.AspNetCore.Mvc;

namespace WebUI.ViewModels.OAuth;

public class ErrorResponseViewModel
{
    [FromQuery(Name = "error")]
    public string Error { get; init; }
    
    [FromQuery(Name = "error_description")]
    public string ErrorDescription { get; init; }
    
    [FromQuery(Name = "error_uri")]
    public string ErrorUri { get; init; } 
}