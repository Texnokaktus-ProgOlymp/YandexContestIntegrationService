using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using RestSharp;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;
using ContestClient = Texnokaktus.ProgOlymp.YandexContestIntegrationService.Client.ContestClient;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient;

public static class DiExtensions
{
    public static IServiceCollection AddYandexClientServices(this IServiceCollection services) =>
        services.AddScoped<IYandexAuthenticationService, YandexAuthenticationService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IAuthenticationProvider, YandexOAuthAuthenticationProvider>()
                .AddScoped<IRequestAdapter, HttpClientRequestAdapter>()
                .AddScoped<ContestClient>()
                .AddScoped<IRestClient>(_ => new RestClient("https://oauth.yandex.ru"));
}
