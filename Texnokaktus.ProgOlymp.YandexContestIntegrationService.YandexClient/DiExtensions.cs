using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Authentication;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;
using YandexContestClient;
using YandexOAuthClient;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient;

public static class DiExtensions
{
    public static IServiceCollection AddYandexClientServices(this IServiceCollection services) =>
        services.AddOAuthClient()
                .StoreWith<CacheTokenStorage>()
                .AddYandexContestClient()
                .AddYandexContestAuthentication<TokenProvider>()
                .AddScoped<ObservabilityOptions>(_ => new());
}
