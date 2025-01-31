using Microsoft.Extensions.Diagnostics.HealthChecks;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.HealthChecks;

public class AuthenticationHealthCheck(ITokenService tokenService) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken) =>
        await tokenService.GetAccessTokenAsync() != null
            ? HealthCheckResult.Healthy("Access token persists")
            : HealthCheckResult.Unhealthy("Application is unauthenticated");
}
