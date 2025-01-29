using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using StackExchange.Redis;
using Texnokaktus.ProgOlymp.Identity;
using Texnokaktus.ProgOlymp.OpenTelemetry;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Consumers;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Options;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddLogicLayerServices()
       .AddIdentityServices(builder.Configuration)
       .AddDataAccess(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb")))
       .AddServiceOptions()
       .AddYandexClientServices();

var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(builder.Configuration.GetConnectionString("DefaultRedis")!);
builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
builder.Services.AddStackExchangeRedisCache(options => options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(connectionMultiplexer));
builder.Services.AddMemoryCache();

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

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddGrpcHealthChecks().AddCheck("Default", () => HealthCheckResult.Healthy());

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddTexnokaktusOpenTelemetry(builder.Configuration, null, null);

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapGrpcHealthChecksService();

app.MapGet("api/contests/{contestId:long}/problems", async(long contestId, IContestClient c) => await c.GetContestProblemsAsync(contestId));
app.MapGet("api/contests/{contestId:long}/standings", async (long contestId, string? participant, IContestClient c) => await c.GetContestStandingsAsync(contestId, forJudge: true, participantSearch: participant));
app.MapGet("api/contests/{contestId:long}/participants/{participantId:long}", async (long contestId, long participantId, IContestClient c) => await c.GetParticipantStatusAsync(contestId, participantId));

app.UseStatusCodePagesWithReExecute("/home/error/{0}");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<ContestDataServiceImpl>();
app.MapGrpcService<RegistrationServiceImpl>();

app.MapControllerRoute(name: "default",
                       pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
