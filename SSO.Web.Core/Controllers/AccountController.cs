using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SSO.Web.Core.Entity;
using SSO.Web.Core.Model.Account;

namespace SSO.Web.Core.Controllers
{
    public class AccountController : Controller
    {
        #region 登录
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            var user = new User();
            if (user.Vaild(loginModel.UserName, loginModel.Password))
            {
                SignIn(user);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "登录失败";
            return View();
        }
        #endregion

        #region 注销
        public IActionResult Logout()
        {
            SignOut();
            return RedirectToAction("Login", "Account");
        }
        #endregion

        #region 私有方法
        private void SignIn(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Id,user.UserId),
                new Claim(JwtClaimTypes.Name,user.UserName),
                new Claim(JwtClaimTypes.NickName,user.RealName),
            };

            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));

            HttpContext.SignInAsync(userPrincipal);
        }

        private void SignOut()
        {
            HttpContext.SignOutAsync();
        }
        #endregion
    }
}
