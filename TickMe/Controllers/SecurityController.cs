using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TickMe.Models;

namespace TickMe.Controllers
{
    public class SecurityController : Controller
    {
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "B2C_1_sign_in");
        }

        [HttpPost]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var scheme = User.FindFirst("tfp").Value;
            await HttpContext.SignOutAsync(scheme);
        }

        [Route("signup")]
        public IActionResult SignUp()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "B2C_1_sign_up");
        }

        [Route("editprofile")]
        public IActionResult EditProfile()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" },"B2C_1_edit_profile");
        }
    }
}