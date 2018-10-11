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
                   options.Cookie.Domain = ".cg.com";
                   options.Events.OnRedirectToLogin = BuildRedirectToLogin;
                   options.Events.OnSigningOut = context =>
                   {
                       var returnUrl = new UriBuilder
                       {
                           Host = context.Request.Host.Host,
                           Port = context.Request.Host.Port ?? 80,
                       };
                       var redirectUrl = new UriBuilder
                       {
                           Host = "sso.cg.com",
                           Path = context.Options.LoginPath,
                           Query = QueryString.Create(context.Options.ReturnUrlParameter, returnUrl.Uri.ToString()).Value
                       };
                       context.Response.Redirect(redirectUrl.Uri.ToString());
                       return Task.CompletedTask;
                   };
                   options.Cookie.HttpOnly = true;
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                   options.LoginPath = "/Account/Login";
                   options.LogoutPath = "/Account/Logout";
                   options.SlidingExpiration = true;
                   //options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"D:\sso\qwe"));
                   options.TicketDataFormat = new TicketDataFormat(new AesDataProtector());
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

        private Task BuildRedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
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
                Host = "sso.cg.com",
                Path = currentUrl.Path,
                Query = QueryString.Create(context.Options.ReturnUrlParameter, returnUrl.Uri.ToString()).Value
            };
            context.Response.Redirect(redirectUrl.Uri.ToString());
            return Task.CompletedTask;
        }

    }
    internal class AesDataProtector : IDataProtector
    {
        public IDataProtector CreateProtector(string purpose)
        {
            return this;
        }

        public byte[] Protect(byte[] plaintext)
        {
            return AESHelper.Encrypt(plaintext, "!@#13487");
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return AESHelper.Decrypt(protectedData, "!@#13487");
        }
    }
}
