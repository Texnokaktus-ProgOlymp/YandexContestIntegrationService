﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth2;
using RestSharp.Serializers.Json;
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
                .AddScoped<IContestClient, Services.ContestClient>()
                .AddScoped<IAuthenticationProvider, YandexOAuthAuthenticationProvider>()
                .AddScoped<IRequestAdapter, HttpClientRequestAdapter>()
                .AddScoped<ContestClient>()
                .AddScoped<IAuthenticator>(provider =>
                 {
                     var tokenService = provider.GetRequiredService<ITokenService>();
                     var accessToken = tokenService.GetAccessTokenAsync()
                                                   .GetAwaiter()
                                                   .GetResult()
                                    ?? throw new("No token");
                     return new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken);
                 })
                .AddKeyedScoped<IRestClient>(ClientType.YandexContest,
                                             (provider, _) =>
                                                 new RestClient("https://api.contest.yandex.net/api/public/v2/",
                                                                options => options.Authenticator = provider.GetRequiredService<IAuthenticator>(),
                                                                configureSerialization: config =>
                                                                    config.UseSystemTextJson(new(JsonSerializerDefaults.Web)
                                                                    {
                                                                        Converters =
                                                                        {
                                                                            new JsonStringEnumConverter()
                                                                        }
                                                                    })))
                .AddKeyedScoped<IRestClient>(ClientType.YandexOAuth, (_, _) => new RestClient("https://oauth.yandex.ru"));
}
