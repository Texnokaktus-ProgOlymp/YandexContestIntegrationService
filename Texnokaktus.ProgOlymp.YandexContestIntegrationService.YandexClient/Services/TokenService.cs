using Microsoft.Extensions.Caching.Distributed;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;

internal class TokenService(IDistributedCache cache,
                            IYandexAuthenticationService yandexAuthenticationService) : ITokenService
{
    private const string AccessTokenKey = "Auth:Yandex:AccessToken";
    private const string RefreshTokenKey = "Auth:Yandex:RefreshToken";

    public async Task RegisterTokenAsync(TokenResponse tokenResponse)
    {
        await cache.SetStringAsync(AccessTokenKey,
                                   tokenResponse.AccessToken,
                                   new DistributedCacheEntryOptions
                                   {
                                       AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.ExpiresIn)
                                   });

        if (tokenResponse.RefreshToken is not null)
            await cache.SetStringAsync(RefreshTokenKey, tokenResponse.RefreshToken);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var accessToken = await cache.GetStringAsync(AccessTokenKey);
        if (accessToken is not null) return accessToken;

        var refreshToken = await cache.GetStringAsync(RefreshTokenKey);
        if (refreshToken is null) return null;

        var response = await yandexAuthenticationService.RefreshAccessTokenAsync(refreshToken);

        await RegisterTokenAsync(response);
        return response.AccessToken;
    }
}
