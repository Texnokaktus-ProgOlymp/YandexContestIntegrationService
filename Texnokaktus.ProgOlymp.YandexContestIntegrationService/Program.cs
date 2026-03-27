using System.Reflection;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.DataProtection;
using Serilog;
using StackExchange.Redis;
using Texnokaktus.ProgOlymp.OpenTelemetry;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Authentication;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;
using YandexContestClient;
using YandexOAuthClient;
using YandexOAuthClient.Diagnostics.HealthChecks;
using YandexOAuthClient.TokenStorage.Decorators.DataProtection;
using YandexOAuthClient.TokenStorage.DistributedCache;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddYandexContestClient()
       .AuthenticateWithTokenProvider<TokenProvider>()
       .WithObservability();

builder.Services
       .AddStoredOAuthClient<string>()
       .WithDistributedCacheStorage(configurator => configurator.ProtectStorage());

var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(builder.Configuration.GetConnectionString("DefaultRedis")!);
builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
builder.Services.AddStackExchangeRedisCache(options => options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(connectionMultiplexer));
builder.Services.AddMemoryCache();

builder.Services
       .AddDefaultAWSOptions(builder.Configuration.GetAWSOptions())
       .AddAWSService<IAmazonS3>()
       .AddScoped<ITransferUtility, TransferUtility>();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services
       .AddHealthChecks()
       .AddAuthenticationHealthCheck(options => options.DefaultTokenKey = "DEFAULT");

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom
                                                                 .Configuration(context.Configuration)
                                                                 .AddOpenTelemetrySupport("YandexContestIntegrationService"));

builder.Services.AddTexnokaktusOpenTelemetry("YandexContestIntegrationService",
                                             providerBuilder => providerBuilder.AddSource("Microsoft.Kiota.Http.HttpClientLibrary"),
                                             null);

builder.Services
       .AddDataProtection(options => options.ApplicationDiscriminator = Assembly.GetEntryAssembly()?.GetName().Name)
       .PersistKeysToStackExchangeRedis(connectionMultiplexer)
       .DisableAutomaticKeyGeneration();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGrpcService<CompilerServiceImpl>();
app.MapGrpcService<ContestDataServiceImpl>();
app.MapGrpcService<ParticipantServiceImpl>();
app.MapGrpcService<RegistrationServiceImpl>();
app.MapGrpcService<SubmissionServiceImpl>();
app.MapGrpcService<YandexAuthenticationServiceImpl>();

await app.RunAsync();
