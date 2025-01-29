using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth2;
using RestSharp.Serializers.Json;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient;

public static class DiExtensions
{
    public static IServiceCollection AddYandexClientServices(this IServiceCollection services) =>
        services.AddScoped<IYandexAuthenticationService, YandexAuthenticationService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IContestClient, ContestClient>()
                .AddScoped<IAuthenticator>(provider =>
                 {
                     var tokenService = provider.GetRequiredService<ITokenService>();
                     var accessToken = tokenService.GetAccessTokenAsync()
                                                   .GetAwaiter()
                                                   .GetResult()
                                    ?? throw new("No token");
                     return new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken);
                 })
                .AddScoped<IRestClient>(provider =>
                 {
                     var authenticator = provider.GetRequiredService<IAuthenticator>();
                     return new RestClient("https://api.contest.yandex.net/api/public/v2/",
                                           options => options.Authenticator = authenticator,
                                           configureSerialization: config => config.UseSystemTextJson(new(JsonSerializerDefaults.Web)
                                               {
                                                   Converters =
                                                   {
                                                       new JsonStringEnumConverter()
                                                   }
                                               }));
                 });
}
