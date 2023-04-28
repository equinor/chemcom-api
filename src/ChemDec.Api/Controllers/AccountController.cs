using System.Threading.Tasks;
using ChemDec.Api.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ChemDec.Api.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserService userService;

        public AccountController(UserService userService)
        {
            this.userService = userService;
        }
        

        [HttpGet]
        public IActionResult LogIn()
        {
            var redirectUrl = "/";// Url.Action(nameof(HomeController.Index), "Home");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserInfo()
        {
            var info = await userService.GetUser(User);
            return Json(info);
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            //var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

       
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden, "You do not have the required authorization to use this resource");
        }
    }
}