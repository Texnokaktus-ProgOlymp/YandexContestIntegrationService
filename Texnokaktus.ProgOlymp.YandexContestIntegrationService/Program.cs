using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Texnokaktus.ProgOlymp.Identity;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Consumers;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Options;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Secrets.json", false);

builder.Services
       .AddLogicLayerServices()
       .AddIdentityServices(builder.Configuration)
       .AddDataAccess(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb")))
       .AddServiceOptions()
       .AddYandexClientServices()
       .AddStackExchangeRedisCache(options => options.Configuration = "raspberrypi.local");

builder.Services.AddControllersWithViews();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services
       .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(x =>
        {
            x.ExpireTimeSpan = TimeSpan.FromMinutes(40);
            x.Cookie.MaxAge = x.ExpireTimeSpan;
            x.SlidingExpiration = true;
            x.LoginPath = "/authentication/login";
            x.LogoutPath = "/authentication/logout";
            x.ReturnUrlParameter = "redirectUrl";
            x.AccessDeniedPath = "/accessDenied";
        });
builder.Services.AddAuthorization();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<ContestStageCreatedConsumer>();

    configurator.UsingRabbitMq((context, factoryConfigurator) =>
    {
        factoryConfigurator.Host(builder.Configuration.GetConnectionString("DefaultRabbitMq"));
        factoryConfigurator.ConfigureEndpoints(context);
    });
});

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/home/error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default",
                       pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
