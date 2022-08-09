using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ApplicationBuildingBlocks.Builder.Authentication;

public static class OpenIdConnectAuthenticationExtensions
{
	public static WebApplicationBuilder AddOpenIdConnectAuthenticationServices(this WebApplicationBuilder builder)
	{
        builder.Services
            .AddAuthentication()
            .AddOpenIdConnect("oidc", "OpenID Connect", options =>
            {
                options.Authority = builder.Configuration["OpenIDConnectAuthentication:Authority"];
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ClientId = builder.Configuration["OpenIDConnectAuthentication:ClientId"];
                options.ClientSecret = builder.Configuration["OpenIDConnectAuthentication:ClientSecret"];
                options.CallbackPath = "/signin-plain-oidc";
                options.SignedOutCallbackPath = "/signout-plain-oidc";
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
            .AddOptions<OpenIdConnectOptions>("oidc")
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
            });            
        
        //builder.Services.ConfigureCookieAuthenticationOptions();
        
        builder.Services.TryAddSingleton<SavedClaimsDictionary>();
            
        builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.ExternalScheme,
            options =>
            {
                options.Events.OnSigningIn = ctx =>
                {
                    return Task.CompletedTask;
                };
            });
        
        builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
            options =>
            {
                options.Events.OnSignedIn = ctx =>
                {
                    return Task.CompletedTask;
                };
                
                options.Events.OnSigningIn = ctx =>
                {
                    return Task.CompletedTask;
                };
            });

        builder.Services
            .AddOptions<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme)
            .Configure<SavedClaimsDictionary>((options, savedClaimsDictionary) =>
            {
                options.Events.OnSigningIn = ctx =>
                {
                    if (ctx.Principal?.Identity is not ClaimsIdentity claimsIdentity 
                        || claimsIdentity.Name == null
                        || !savedClaimsDictionary.TryGetValue(claimsIdentity.Name, out var savedClaims))
                    {
                        return Task.CompletedTask;
                    }

                    var newClaimsIdentity = new ClaimsIdentity(savedClaims, IdentityConstants.ExternalScheme);
                    ctx.Principal.AddIdentity(newClaimsIdentity);
                    savedClaimsDictionary.Remove(claimsIdentity.Name);
                    return Task.CompletedTask;
                };
            });
        
        return builder;
	}
}