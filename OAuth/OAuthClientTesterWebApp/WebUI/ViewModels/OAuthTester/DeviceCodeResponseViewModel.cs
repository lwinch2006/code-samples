using System.ComponentModel;

namespace WebUI.ViewModels.OAuthTester;

public class DeviceCodeResponseViewModel
{
    [DisplayName("User code")]
    public string UserCode { get; init; }
    
    [DisplayName("Verification URL")]
    public string VerificationUri { get; init; }
    
    [DisplayName("Interval")]
    public int Interval { get; init; }
    
    [DisplayName("User code will expire on")]
    public DateTime Expires { get; init; }
}