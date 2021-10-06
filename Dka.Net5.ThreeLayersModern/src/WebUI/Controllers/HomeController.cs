using System;
using System.Threading.Tasks;
using Dka.Net5.IdentityWithDapper.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Logout1(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            //_logger.LogInformation(LoggerEventIds.UserLoggedOut, "User logged out.");
            
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Logout2(string returnUrl = null)
        {
            string scheme = null;
            
            if (AppServicesAuthenticationInformation.IsAppServicesAadAuthenticationEnabled)
            {
                return LocalRedirect(AppServicesAuthenticationInformation.LogoutUrl);
            }
            else
            {
                scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
                var callbackUrl = Url.Page("/MicrosoftIdentity/Account/SignedOut", pageHandler: null, values: null, protocol: Request.Scheme);
                return SignOut(
                    new AuthenticationProperties
                    {
                        RedirectUri = callbackUrl,
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    scheme);
            }
        }

        public IActionResult GetError()
        {
            throw new Exception("Generated exception");
        }

        public IActionResult GetNotFound()
        {
            return NotFound();
        }
    }
}