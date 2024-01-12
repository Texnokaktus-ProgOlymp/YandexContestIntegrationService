using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Options.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Options;

public static class DiExtensions
{
    public static IServiceCollection AddServiceOptions(this IServiceCollection services)
    {
        services.AddOptions<YandexAppParameters>().BindConfiguration(nameof(YandexAppParameters));

        return services;
    }
}
