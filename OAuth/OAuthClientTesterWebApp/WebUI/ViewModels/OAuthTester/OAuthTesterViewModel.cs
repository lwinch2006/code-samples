using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OAuthClient.Models;
using WebUI.ViewModels.OAuth;

namespace WebUI.ViewModels.OAuthTester;

public class OAuthTesterViewModel
{
    [Required]
    [DisplayName("Configuration name")]
    public string ConfigurationName { get; init; }
    
    [DisplayName("Username")]
    public string Username { get; init; }
    
    [DisplayName("Password")]
    [DataType(DataType.Password)]
    public string Password { get; init; }
    
    public OAuthClientConfiguration OAuthClientConfiguration { get; set; }

    public AccessTokenResponseViewModel AccessTokenResponse { get; set; }

    public IDictionary<string, string> AvailableConfigurationNames { get; init; }

    [DisplayName("User info")]
    public string UserInfo { get; set; }
}