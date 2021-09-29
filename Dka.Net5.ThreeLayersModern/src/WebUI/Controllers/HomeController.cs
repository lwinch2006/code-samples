using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            string scheme = null;
            
            if (AppServicesAuthenticationInformation.IsAppServicesAadAuthenticationEnabled)
            {
                return LocalRedirect(AppServicesAuthenticationInformation.LogoutUrl);
            }
            else
            {
                scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
                var callbackUrl = Url.Page("/Account/SignedOut", pageHandler: null, values: null, protocol: Request.Scheme);
                return SignOut(
                    new AuthenticationProperties
                    {
                        RedirectUri = callbackUrl,
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    scheme);
            }
            
            
            
            
            
            
            
            
            
            
            return View();
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