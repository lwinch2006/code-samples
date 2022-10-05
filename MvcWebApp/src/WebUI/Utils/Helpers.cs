namespace WebUI.Utils;

public static class Helpers
{
    public static void CheckDebugInfo(IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();
        
        var schemeProvider = sp.GetRequiredService<IAuthenticationSchemeProvider>();
        var handlerProvider = sp.GetRequiredService<IAuthenticationHandlerProvider>();
        var authService = sp.GetRequiredService<IAuthenticationService>();

        var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
        
        var schemes = schemeProvider.GetAllSchemesAsync().GetAwaiter().GetResult();

        var requestHandlerSchemes = schemeProvider.GetRequestHandlerSchemesAsync().GetAwaiter().GetResult();

        var cookieOptions1 = optionsMonitor.Get(IdentityConstants.ApplicationScheme);
        var cookieOptions2 = optionsMonitor.Get(IdentityConstants.ExternalScheme);
        
        var test = 0;
    } 
}