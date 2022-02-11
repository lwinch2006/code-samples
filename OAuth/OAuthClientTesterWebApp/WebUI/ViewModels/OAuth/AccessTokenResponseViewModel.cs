using System.ComponentModel;

namespace WebUI.ViewModels.OAuth;

public class AccessTokenResponseViewModel
{
    [DisplayName("Access token")]
    public string AccessToken { get; init; }
    
    [DisplayName("Token type")]
    public string TokenType { get; init; }
    
    [DisplayName("Expires in")]
    public int ExpiresIn { get; init; }
    
    [DisplayName("Refresh token")]
    public string RefreshToken { get; init; }
    
    [DisplayName("Scope")]
    public string Scope { get; init; }
}