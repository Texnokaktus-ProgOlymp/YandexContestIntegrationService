using Amazon.S3;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware.Options;
using YandexContestClient.Client;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests.Tools;

public sealed class CustomWebApplicationFactory(string redisConnectionString) : WebApplicationFactory<Program>
{
    public ControlledUpstream Upstream { get; } = new();
    public ControlledStorage Storage { get; } = new();
    private HttpClientRequestAdapter? adapter;
    private HttpClient? upstreamHttpClient;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // The application opens Redis before Build(), so ConfigureTestServices is too late.
        builder.ConfigureHostConfiguration(configuration => configuration.AddInMemoryCollection(
                                               new Dictionary<string, string?>
                                               {
                                                   ["ConnectionStrings:DefaultRedis"] = redisConnectionString
                                               }
                                           )
        );

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<ContestClient>();
                upstreamHttpClient = KiotaClientFactory.Create(Upstream, [new RetryHandlerOption { MaxRetry = 0 }]);
                adapter = new(new AnonymousAuthenticationProvider(), httpClient: upstreamHttpClient);
                services.AddSingleton(new ContestClient(adapter));
                services.RemoveAll<IAmazonS3>();
                services.AddSingleton(Storage.Client);
            }
        );
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        adapter?.Dispose();
        upstreamHttpClient?.Dispose();
    }
}
