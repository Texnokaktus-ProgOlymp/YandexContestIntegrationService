using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;

internal class TokenService(IDistributedCache cache,
                            IYandexAuthenticationService yandexAuthenticationService,
                            IDataProtectionProvider dataProtectionProvider,
                            ILogger<TokenService> logger) : ITokenService
{
    private const string AccessTokenKey = "Auth:Yandex:AccessToken:Dev";
    private const string RefreshTokenKey = "Auth:Yandex:RefreshToken:Dev";

    private readonly IDataProtector _accessTokenProtector = dataProtectionProvider.CreateProtector("contest:oauth:access-token");
    private readonly IDataProtector _refreshTokenProtector = dataProtectionProvider.CreateProtector("contest:oauth:refresh-token");

    public async Task RegisterTokenAsync(TokenResponse tokenResponse)
    {
        await SetCachedToken(tokenResponse.AccessToken,
                             AccessTokenKey,
                             _accessTokenProtector,
                             new()
                             {
                                 AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.ExpiresIn)
                             });

        if (tokenResponse.RefreshToken is not null)
            await SetCachedToken(tokenResponse.RefreshToken, RefreshTokenKey, _refreshTokenProtector, new());
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var accessToken = await GetCachedAccessToken();
        if (accessToken is not null) return accessToken;

        var refreshToken = await GetCachedRefreshToken();
        if (refreshToken is null) return null;

        var response = await yandexAuthenticationService.RefreshAccessTokenAsync(refreshToken);

        await RegisterTokenAsync(response);
        return response.AccessToken;
    }

    private async Task<string?> GetCachedAccessToken()
    {
        try
        {
            return await GetCachedToken(AccessTokenKey, _accessTokenProtector);
        }
        catch (CryptographicException e)
        {
            logger.LogError(e, "Unable to unprotect access token");
            return null;
        }
    }

    private async Task<string?> GetCachedRefreshToken()
    {
        try
        {
            return await GetCachedToken(RefreshTokenKey, _refreshTokenProtector);
        }
        catch (CryptographicException e)
        {
            logger.LogError(e, "Unable to unprotect refresh token");
            return null;
        }
    }

    private async Task<string?> GetCachedToken(string tokenKey, IDataProtector protector) =>
        await cache.GetStringAsync(tokenKey) is not { } cached
            ? null
            : protector.Unprotect(cached);

    private Task SetCachedToken(string token, string key, IDataProtector dataProtector, DistributedCacheEntryOptions options) =>
        cache.SetStringAsync(key, dataProtector.Protect(token), options);
}
