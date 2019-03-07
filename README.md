# Core.SSO
It is sso demo on .net core.Include same domain and cross domain.

## Same Domain
In the Same parent domain,use share cookies.The key lies in set cookie option.

```c#
public void ConfigureServices(IServiceCollection services)
{
       services.AddMvc();
       services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options =>
        {
               options.Cookie.Name = "Token";
               options.Cookie.Domain = ".cg.com";
               options.Cookie.HttpOnly = true;
               options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
               options.LoginPath = "/Account/Login";
               options.LogoutPath = "/Account/Logout";
               options.SlidingExpiration = true;
               //options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"D:\sso\key"));
               options.TicketDataFormat = new TicketDataFormat(new AesDataProtector());
        });
}
```

## cross domain
The key lies in jump pages.
