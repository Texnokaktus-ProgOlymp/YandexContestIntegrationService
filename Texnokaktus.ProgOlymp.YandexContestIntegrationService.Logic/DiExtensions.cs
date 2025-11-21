using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Authentication;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using YandexContestClient;
using YandexOAuthClient;
using YandexOAuthClient.TokenStorage.Decorators.DataProtection;
using YandexOAuthClient.TokenStorage.DistributedCache;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic;

public static class DiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddLogicLayerServices() =>
            services.AddScoped<IRegistrationService, RegistrationService>()
                    .AddScoped<IParticipantService, ParticipantService>();

        public IServiceCollection AddYandexClientServices()
        {
            services.AddYandexContestClient()
                    .AuthenticateWithTokenProvider<TokenProvider>()
                    .WithObservability();

            services.AddOAuthClient()
                    .WithDistributedCacheStorage(configurator => configurator.ProtectStorage());

            return services;
        }
    }
}


