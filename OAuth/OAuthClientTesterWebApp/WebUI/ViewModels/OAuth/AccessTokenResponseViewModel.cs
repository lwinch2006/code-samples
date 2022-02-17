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
    
    [DisplayName("ID token")]
    public string IdToken { get; init; }

    [DisplayName("Extended expires in")]
    public int ExtendedExpiresIn { get; init; }
}