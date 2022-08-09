using ApplicationBuildingBlocks.Builder.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace WebUI.ApplicationSetupExtensions.Builder.Authentication;

public static class OktaAuthenticationExtensions
{
	public static WebApplicationBuilder AddOktaAuthenticationServices(this WebApplicationBuilder builder)
	{
        builder.Services
            .AddAuthentication()
            .AddOpenIdConnect("okta", "Okta", options =>
            {
                options.Authority = builder.Configuration["OktaAuthentication:Authority"];
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ClientId = builder.Configuration["OktaAuthentication:ClientId"];
                options.ClientSecret = builder.Configuration["OktaAuthentication:ClientSecret"];
                options.CallbackPath = "/signin-okta-oidc";
                options.SignedOutCallbackPath = "/signout-okta-oidc";
                options.SignInScheme = IdentityConstants.ExternalScheme;
                
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("address");
                options.Scope.Add("phone");
                
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;
                options.ClaimActions.MapAll();
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "preferred_username",
                    RoleClaimType = "groups",
                    ValidateIssuer = true
                };
            });
            
        builder.Services
            .AddOptions<OpenIdConnectOptions>("okta")
            .Configure<SavedClaimsDictionary>((options, savedClaimsDictionary) =>
            {
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

                options.Events.OnRedirectToIdentityProviderForSignOut = ctx =>
                {
                    var idToken = ctx.HttpContext.User.GetIdTokens().Single();
                    ctx.ProtocolMessage.IdTokenHint = idToken;
                    return Task.CompletedTask;
                };
            });            
            
            //builder.Services.ConfigureCookieAuthenticationOptions();            
		
		return builder;
	}
}