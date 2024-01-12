using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

public interface IYandexAuthenticationService
{
    string GetYandexOAuthUrl(string? localRedirectUri);
    Task<TokenResponse> GetAccessTokenAsync(string code);
    Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken);
}
