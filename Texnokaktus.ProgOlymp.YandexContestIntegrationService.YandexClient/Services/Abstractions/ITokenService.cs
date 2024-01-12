using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

public interface ITokenService
{
    Task RegisterTokenAsync(TokenResponse tokenResponse);
    Task<string?> GetAccessTokenAsync();
}
