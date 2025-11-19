using Microsoft.Extensions.Diagnostics.HealthChecks;
using YandexOAuthClient.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.HealthChecks;

public class AuthenticationHealthCheck(IAuthService tokenService) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken) =>
        await tokenService.GetAccessTokenAsync("DEFAULT") != null
            ? HealthCheckResult.Healthy("Access token persists")
            : HealthCheckResult.Unhealthy("Application is unauthenticated");
}
