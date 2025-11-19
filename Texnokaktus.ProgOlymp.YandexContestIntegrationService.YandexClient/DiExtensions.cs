using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Authentication;
using YandexContestClient;
using YandexOAuthClient;
using YandexOAuthClient.TokenStorage.Decorators.DataProtection;
using YandexOAuthClient.TokenStorage.DistributedCache;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient;

public static class DiExtensions
{
    public static IServiceCollection AddYandexClientServices(this IServiceCollection services)
    {
        services.AddYandexContestClient()
                .AuthenticateWithTokenProvider<TokenProvider>()
                .WithObservability();

        services.AddOAuthClient()
                .WithDistributedCacheStorage(configurator => configurator.ProtectStorage());

        return services;
    }
}
