using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SSO.Web.Core.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(HttpContext.User.Claims.Select(c => new List<string> { c.Type, c.Value }).ToList());
        }
    }
}
