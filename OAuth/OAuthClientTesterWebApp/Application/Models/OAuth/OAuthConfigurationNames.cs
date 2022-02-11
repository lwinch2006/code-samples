using System.ComponentModel;

namespace Application.Models.OAuth;

public static class OAuthConfigurationNames
{
    [Description("Github")]
    public const string Github = "Github";
    
    [Description("Authorization code")]
    public const string AuthorizationCode = "AuthorizationCode";
    
    [Description("Authorization code with PKCE")]
    public const string AuthorizationCodeWithPKCE = "AuthorizationCodeWithPKCE";
    
    [Description("Implicit")]
    public const string Implicit = "Implicit";
    
    [Description("Password")]
    public const string Password = "Password";
    
    [Description("Client credentials")]
    public const string ClientCredentials = "ClientCredentials";
    
    [Description("Device")]
    public const string Device = "Device";
}