using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace ApplicationBuildingBlocks.Builder.Authentication;

public static class MicrosoftAuthenticationExtensions
{
	public static WebApplicationBuilder AddMicrosoftAuthenticationServices(this WebApplicationBuilder builder)
	{
		builder
			.AddMicrosoftAuthentication()
			.AddAzureAd()
			.AddAzureAdUi();
		
		return builder;
	}
	
	public static WebApplicationBuilder AddMicrosoftAuthentication(this WebApplicationBuilder builder)
	{
		builder.Services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
		{
			microsoftOptions.ClientId = builder.Configuration["MicrosoftAuthentication:ClientId"];
			microsoftOptions.ClientSecret = builder.Configuration["MicrosoftAuthentication:ClientSecret"];
		});
            
		builder.Services.Configure<MicrosoftAccountOptions>(MicrosoftAccountDefaults.AuthenticationScheme, options =>
		{
			options.SaveTokens = true;
                
			options.Events.OnTicketReceived = ctx =>
			{
				return Task.CompletedTask;
			};
		});

		builder.Services.ConfigureCookieAuthenticationOptions();

		return builder;
	}
	
    public static WebApplicationBuilder AddAzureAd(this WebApplicationBuilder builder)
    {
        builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd", displayName: "Azure AD", subscribeToOpenIdConnectMiddlewareDiagnosticsEvents: true);

        builder.Services
            .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
            .Configure<SavedClaimsDictionary>((options, savedClaimsDictionary) =>
            {
                options.SignInScheme = IdentityConstants.ExternalScheme;
            
                options.Events.OnSignedOutCallbackRedirect += ctx =>
                {
                    return Task.CompletedTask;
                };

                options.Events.OnRemoteSignOut += ctx =>
                {
                    return Task.CompletedTask;
                };

                options.ClaimActions.MapAll();
            
                options.Events.OnTicketReceived = ctx =>
                {
                    if (ctx.Principal?.Identity is not ClaimsIdentity claimsIdentity || 
                        claimsIdentity.Name == null)
                    {
                        return Task.CompletedTask;
                    }

                    if (savedClaimsDictionary.ContainsKey(claimsIdentity.Name))
                    {
                        savedClaimsDictionary[claimsIdentity.Name].AddRange(ctx.Principal.Claims);
                    }
                    else
                    {
                        savedClaimsDictionary.Add(claimsIdentity.Name, ctx.Principal.Claims.ToList());
                    }
                    
                    if (ctx.Properties?.GetTokenValue("access_token") is { } accessToken)
                    {
                        savedClaimsDictionary[claimsIdentity.Name].Add(new Claim("access_token", accessToken));
                    }
                    
                    if (ctx.Properties?.GetTokenValue("id_token") is { } idToken)
                    {
                        savedClaimsDictionary[claimsIdentity.Name].Add(new Claim("id_token", idToken));
                    }                        

                    return Task.CompletedTask;
                };
            });

        builder.Services.ConfigureCookieAuthenticationOptions();
        
        return builder;
    }
	
	public static WebApplicationBuilder AddAzureAdUi(this WebApplicationBuilder builder)
	{
		builder.Services
			.AddMvc()
			.AddMicrosoftIdentityUI();
		return builder;
	}
}