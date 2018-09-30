using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSO.Web.Core.Helper;

namespace SSO.Web.Core
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
#if !DEBUG
                   options.Cookie.Domain = ".sso.com";
#endif
                   options.Cookie.HttpOnly = true;
                   options.Cookie.Expiration = DateTime.Now.AddMinutes(30).TimeOfDay;
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
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
                    "{controller=Account}/{action=Login}/{id?}");
            });
        }

        private class AesDataProtector : IDataProtector
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
}
