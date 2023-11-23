using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Identity;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddIdentityServices(builder.Configuration)
       .AddDataAccess(optionsBuilder => optionsBuilder.UseSqlServer("data source=raspberrypi.local;initial catalog=YandexContestIntegration;User Id=sa;Password=Raspberry-Pi;Encrypt=True;TrustServerCertificate=True"));

builder.Services.AddControllersWithViews();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services
       .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(x =>
        {
            x.ExpireTimeSpan = TimeSpan.FromMinutes(40);
            x.Cookie.MaxAge = x.ExpireTimeSpan;
            x.SlidingExpiration = true;
            x.LoginPath = "/authentication";
            x.LogoutPath = "/authentication/logout";
            x.ReturnUrlParameter = "redirectUrl";
            x.AccessDeniedPath = "/accessDenied";
        });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default",
                       pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
