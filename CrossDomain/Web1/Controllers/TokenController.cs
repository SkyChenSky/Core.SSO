using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Model.Token;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Web1.Controllers
{
    public class TokenController : Controller
    {
        public static TokenCookieOptions CookieOptions { get; set; }

        public IActionResult Authorization(string token, List<string> hostAuthorization = null)
        {
            if (CookieOptions == null || string.IsNullOrEmpty(token))
                return BadRequest();

            HttpContext.Response.Cookies.Append(CookieOptions.Name, token, new CookieOptions
            {
                Domain = CookieOptions.Domain,
                Expires = DateTimeOffset.UtcNow.Add(CookieOptions.Expires),
                HttpOnly = CookieOptions.HttpOnly,
                IsEssential = CookieOptions.IsEssential,
                MaxAge = CookieOptions.MaxAge,
                Path = CookieOptions.Path,
                SameSite = CookieOptions.SameSite
            });

            if (hostAuthorization.Any())
                hostAuthorization = hostAuthorization.Where(a => !a.Contains(HttpContext.Request.Host.Host)).ToList();

            if (!hostAuthorization.Any())
                hostAuthorization = new List<string> { "http://www.sso.com" };

            return View(new TokenViewData
            {
                Token = token,
                HostAuthorization = hostAuthorization
            });
        }

        public IActionResult Logout(List<string> hostAuthorization = null)
        {
            HttpContext.Response.Cookies.Delete(CookieOptions.Name);

            if (hostAuthorization.Any())
                hostAuthorization = hostAuthorization.Where(a => !a.Contains(HttpContext.Request.Host.Host)).ToList();

            if (!hostAuthorization.Any())
                hostAuthorization = new List<string> { "http://www.sso.com" };

            return View(new TokenViewData
            {
                HostAuthorization = hostAuthorization
            });
        }
    }

    public class TokenCookieOptions
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public TimeSpan Expires { get; set; }
        public bool HttpOnly { get; set; }
        public bool IsEssential { get; set; }
        public TimeSpan? MaxAge { get; set; }
        public string Path { get; set; }
        public SameSiteMode SameSite { get; set; }
    }
}
