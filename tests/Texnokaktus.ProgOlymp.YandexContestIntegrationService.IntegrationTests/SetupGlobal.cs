using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Testcontainers.Redis;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests.Tools;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests;

[SetUpFixture]
public sealed class SetupGlobal
{
    public static CustomWebApplicationFactory Factory { get; private set; } = null!;
    private RedisContainer? redis;
    private IConnectionMultiplexer? connection;
    private string? previousServiceName;

    [OneTimeSetUp]
    public async Task Start()
    {
        previousServiceName = Environment.GetEnvironmentVariable("SERVICE_NAME");
        Environment.SetEnvironmentVariable("SERVICE_NAME", "YandexContestIntegrationService.IntegrationTests");
        redis = new RedisBuilder("redis:7.4-alpine").Build();
        await redis.StartAsync();
        Factory = new(redis.GetConnectionString());
        connection = Factory.Services.GetRequiredService<IConnectionMultiplexer>();
    }

    [OneTimeTearDown]
    public async Task Stop()
    {
        try
        {
            if (Factory is not null)
                await Factory.DisposeAsync();

            if (connection is not null)
                await connection.DisposeAsync();

            if (redis is not null)
                await redis.DisposeAsync();
        }
        finally
        {
            Environment.SetEnvironmentVariable("SERVICE_NAME", previousServiceName);
        }
    }
}
