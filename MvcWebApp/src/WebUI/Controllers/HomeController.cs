using System.Web;
using ApplicationBuildingBlocks.Builder.Authentication;
using IdentityWithDapper.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
                //var callbackUrl = Url.Page("/MicrosoftIdentity/Account/SignedOut", pageHandler: null, values: null, protocol: Request.Scheme);

                var baseUrl = new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}");
                var callbackUrl = new Uri(baseUrl, "MicrosoftIdentity/Account/SignedOut");
                
                return SignOut(
                    new AuthenticationProperties
                    {
                        RedirectUri = callbackUrl.ToString(),
                    },
                    IdentityConstants.ExternalScheme,
                    scheme);
            }
        }

        public async Task Logout3(string returnUrl = null)
        {
            var baseUrl = new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}");
            var callbackUrl = new Uri(baseUrl, "MicrosoftIdentity/Account/SignedOut");
            var options = new AuthenticationProperties
            {
                RedirectUri = callbackUrl.ToString(),
            };
            
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, options);
        }

        public async Task<IActionResult> Logout5()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);
            
            return Redirect($"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={HttpUtility.UrlEncode("https://localhost:5556")}");
        }
        
        public IActionResult Logout4()
        {
            var redirectUrl = "/";

            var signOutSchemes = new[]
            {
                IdentityConstants.ApplicationScheme,
                IdentityConstants.ExternalScheme,
                IdentityConstants.TwoFactorUserIdScheme,
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
            };
            
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = redirectUrl
                },
                signOutSchemes);
        }        
        
        public IActionResult Logout6()
        {
            var redirectUrl = "/";
            
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
                    RedirectUri = redirectUrl
                },
                signOutSchemes.ToArray());
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