using System.Reflection;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using Texnokaktus.ProgOlymp.OpenTelemetry;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.HealthChecks;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;
using YandexOAuthClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddLogicLayerServices()
       .AddDataAccess(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb"))
                                                      .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()))
       .AddYandexClientServices()
       .AddOAuthClient();

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
       .AddGrpcHealthChecks()
       .AddCheck<AuthenticationHealthCheck>(nameof(AuthenticationHealthCheck))
       .AddDatabaseHealthChecks();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddTexnokaktusOpenTelemetry("YandexContestIntegrationService",
                                             providerBuilder => providerBuilder.AddSource("Microsoft.Kiota.Http.HttpClientLibrary"),
                                             null);

builder.Services
       .AddDataProtection(options => options.ApplicationDiscriminator = Assembly.GetEntryAssembly()?.GetName().Name)
       .PersistKeysToStackExchangeRedis(connectionMultiplexer);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGrpcHealthChecksService();

app.MapGrpcService<CompilerServiceImpl>();
app.MapGrpcService<ContestDataServiceImpl>();
app.MapGrpcService<ParticipantServiceImpl>();
app.MapGrpcService<RegistrationServiceImpl>();
app.MapGrpcService<SubmissionServiceImpl>();
app.MapGrpcService<YandexAuthenticationServiceImpl>();

await app.RunAsync();
