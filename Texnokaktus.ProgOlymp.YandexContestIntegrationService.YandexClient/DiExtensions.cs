using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Http.HttpClientLibrary;
using RestSharp;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Authentication;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;
using YandexContestClient;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient;

public static class DiExtensions
{
    public static IServiceCollection AddYandexClientServices(this IServiceCollection services) =>
        services.AddScoped<IYandexAuthenticationService, YandexAuthenticationService>()
                .AddScoped<ITokenService, TokenService>()
                .AddYandexContestAuthentication<TokenProvider>()
                .AddYandexContestClient()
                .AddScoped<ObservabilityOptions>(_ => new())
                .AddScoped<IRestClient>(_ => new RestClient("https://oauth.yandex.ru"));
}
