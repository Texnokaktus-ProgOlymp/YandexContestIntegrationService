using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Authentication;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;
using YandexContestClient;
using YandexOAuthClient;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient;

public static class DiExtensions
{
    public static IServiceCollection AddYandexClientServices(this IServiceCollection services)
    {
        services.AddYandexContestClient()
                .AuthenticateWithTokenProvider<TokenProvider>()
                .WithObservability();

        services.AddOAuthClient()
                .WithTokenStorage<CacheTokenStorage>(configurator => configurator.WithDecorator<EncryptedStorageDecorator>());

        return services;
    }
}
