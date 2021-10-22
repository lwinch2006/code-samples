using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.Utils.Extensions;

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

            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = returnUrl
                },
                signOutSchemes.ToArray());
        }
    }
}