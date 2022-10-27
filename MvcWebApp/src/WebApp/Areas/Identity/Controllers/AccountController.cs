using Application.Models;
using ApplicationBuildingBlocks.Builder.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Areas.Identity.Controllers
{
	[Area("Identity")]
	[Authorize]
	public class AccountController : Controller
	{
		[HttpPost]
		public IActionResult SignOut(string returnUrl)
		{
			var signOutSchemes = new List<string>
			{
				IdentityConstants.ApplicationScheme,
				IdentityConstants.ExternalScheme,
				IdentityConstants.TwoFactorUserIdScheme
			};

			var authenticationProperties = new AuthenticationProperties
			{
				RedirectUri = returnUrl
			};

			if (HttpContext.IsMicrosoftAccountLogin() || HttpContext.IsAzureAdLogin())
			{
				signOutSchemes.AddRange(new[]
					{ CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme });
			}

			if (HttpContext.IsAuthenticatedWithScheme(ApplicationConstants.Authentication.Schemes.OIDC))
			{
				signOutSchemes.Add(ApplicationConstants.Authentication.Schemes.OIDC);
			}

			if (HttpContext.IsAuthenticatedWithScheme(ApplicationConstants.Authentication.Schemes.Okta))
			{
				signOutSchemes.Add(ApplicationConstants.Authentication.Schemes.Okta);
				authenticationProperties.SetString("client_id", "0oa495nn7pRnQoGHZ5d7");
			}

			return SignOut(
				authenticationProperties,
				signOutSchemes.ToArray());
		}
	}
}