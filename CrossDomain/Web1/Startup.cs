using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web1.Controllers;
using Web1.Helper;

namespace Web1
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
               {
                   options.Cookie.Name = "Token";
                   options.Events.OnRedirectToLogin = BuildRedirectToLogin;
                   options.Events.OnSigningOut = BuildSigningOut;
                   options.Cookie.HttpOnly = true;
                   options.Cookie.Path = "/";
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                   options.LoginPath = "/Account/Login";
                   options.LogoutPath = "/Account/Logout";
                   options.SlidingExpiration = true;
                   options.TicketDataFormat = new TicketDataFormat(new AesDataProtector());

                   TokenController.CookieOptions = new TokenCookieOptions
                   {
                       Name = options.Cookie.Name,
                       Domain = options.Cookie.Domain,
                       Expires = options.ExpireTimeSpan,
                       HttpOnly = options.Cookie.HttpOnly,
                       IsEssential = options.Cookie.IsEssential,
                       MaxAge = options.Cookie.MaxAge,
                       Path = options.Cookie.Path,
                       SameSite = options.Cookie.SameSite,
                   };
               });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// 未登录下，引导跳转认证中心登录页面
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Task BuildRedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            var currentUrl = new UriBuilder(context.RedirectUri);
            var returnUrl = new UriBuilder
            {
                Host = currentUrl.Host,
                Port = currentUrl.Port,
                Path = context.Request.Path
            };
            var redirectUrl = new UriBuilder
            {
                Host = "www.sso.com",
                Path = currentUrl.Path,
                Query = QueryString.Create(context.Options.ReturnUrlParameter, returnUrl.Uri.ToString()).Value
            };
            context.Response.Redirect(redirectUrl.Uri.ToString());
            return Task.CompletedTask;
        }

        /// <summary>
        /// 注销，引导跳转认证中心登录页面
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Task BuildSigningOut(CookieSigningOutContext context)
        {
            var returnUrl = new UriBuilder
            {
                Host = context.Request.Host.Host,
                Port = context.Request.Host.Port ?? 80,
            };
            var redirectUrl = new UriBuilder
            {
                Host = "www.sso.com",
                Path = context.Options.LoginPath,
                Query = QueryString.Create(context.Options.ReturnUrlParameter, returnUrl.Uri.ToString()).Value
            };
            context.Response.Redirect(redirectUrl.Uri.ToString());
            return Task.CompletedTask;
        }
    }
    internal class AesDataProtector : IDataProtector
    {
        private const string Key = "!@#13487";

        public IDataProtector CreateProtector(string purpose)
        {
            return this;
        }

        public byte[] Protect(byte[] plaintext)
        {
            return AESHelper.Encrypt(plaintext, Key);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return AESHelper.Decrypt(protectedData, Key);
        }
    }
}
