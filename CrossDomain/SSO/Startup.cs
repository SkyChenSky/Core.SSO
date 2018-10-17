using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSO.Controllers;
using SSO.Helper;

namespace SSO
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
                   options.Cookie.HttpOnly = true;
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                   options.LoginPath = "/Account/Login";
                   options.LogoutPath = "/Account/Logout";
                   options.SlidingExpiration = true;
                   //options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"D:\sso\key"));
                   options.TicketDataFormat = new TicketDataFormat(new AesDataProtector());
                   TokenController.CookieName = options.Cookie.Name;
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
