using Application.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Areas.Identity.Controllers
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

            if (HttpContext.IsMicrosoftAccountLogin() || HttpContext.IsAzureAdLogin())
            {
                signOutSchemes.AddRange(new[] {CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme});
            }

            if (HttpContext.IsAuthenticatedWithScheme(ApplicationConstants.Authentication.Schemes.OIDC))
            {
                signOutSchemes.Add(ApplicationConstants.Authentication.Schemes.OIDC);
            }

            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = returnUrl
                },
                signOutSchemes.ToArray());
        }
    }
}