using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebUI.ApplicationSetupExtensions.Builder.Authentication;

namespace ApplicationBuildingBlocks.Builder.Authentication;

public static class WebApplicationBuilderAuthenticationExtensions
{
	public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
	{
		return builder;
	}
	
	public static WebApplicationBuilder AddAuthenticationServicesFromProviders(this WebApplicationBuilder builder)
	{
		builder
			.AddMicrosoftAuthenticationServices()
			.AddOpenIdConnectAuthenticationServices()
			.AddTwitterAuthenticationServices()
			.AddOktaAuthenticationServices();
		
		return builder;
	}
	
	public static IServiceCollection ConfigureCookieAuthenticationOptions(this IServiceCollection services)
	{
		services.TryAddSingleton<SavedClaimsDictionary>();
            
		services.Configure<CookieAuthenticationOptions>(IdentityConstants.ExternalScheme,
			options =>
			{
				options.Events.OnSigningIn = ctx =>
				{
					return Task.CompletedTask;
				};
			});

		services
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

		return services;
	}
}