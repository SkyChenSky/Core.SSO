using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Entity;
using Web1.Model.Account;

namespace Web1.Controllers
{
    public class AccountController : Controller
    {
        private const string ReturnUrlKey = "ReturnUrl";
        #region 登录
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                HttpContext.Response.Cookies.Append(ReturnUrlKey, returnUrl, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax
                });

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var user = new User();
            if (user.Vaild(loginModel.UserName, loginModel.Password))
                return await SignIn(user);

            ViewBag.Error = "登录失败";
            return View();
        }
        #endregion

        #region 注销
        public async Task Logout()
        {
            await SignOut();
        }
        #endregion

        #region 私有方法
        private async Task<IActionResult> SignIn(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Id,user.UserId),
                new Claim(JwtClaimTypes.Name,user.UserName),
                new Claim(JwtClaimTypes.NickName,user.RealName),
            };

            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));

            var returnUrl = HttpContext.Request.Cookies[ReturnUrlKey];
            await HttpContext.SignInAsync(userPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    RedirectUri = returnUrl
                });

            HttpContext.Response.Cookies.Delete(ReturnUrlKey);

            return Redirect(returnUrl ?? "/");
        }

        private async Task SignOut()
        {
            await HttpContext.SignOutAsync();
        }
        #endregion
    }
}
