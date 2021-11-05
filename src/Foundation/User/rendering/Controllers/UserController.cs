using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Okta.AspNetCore;

namespace Mvp.Foundation.Users.Rendering.Controllers
{
    public class UserController : Controller
    {
        public IActionResult SignIn()
        {
            // return View();

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                var properties = new AuthenticationProperties
                {
                    RedirectUri = "/"
                };

                return Challenge(properties, OktaDefaults.MvcAuthenticationScheme);
            }

            return Redirect("/");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SignIn([FromForm]string sessionToken)
        //{
        //    if (!HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        var properties = new AuthenticationProperties();
        //        properties.Items.Add("sessionToken", sessionToken);
        //        properties.RedirectUri = "/";

        //        return Challenge(properties, OktaDefaults.MvcAuthenticationScheme);
        //    }

        //    return Redirect("/");
        //}

        public IActionResult Claims()
        {
            // var userClaims = HttpContext.User.Claims;

            return View();
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            return new SignOutResult(
                new[]
                {
                     OktaDefaults.MvcAuthenticationScheme,
                     CookieAuthenticationDefaults.AuthenticationScheme,
                },
                new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}