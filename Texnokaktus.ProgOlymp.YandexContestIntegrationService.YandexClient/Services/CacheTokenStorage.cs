using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using YandexOAuthClient;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;

internal class CacheTokenStorage(IDistributedCache cache, IDataProtectionProvider dataProtectionProvider) : ITokenStorage
{
    private static string GetCacheKey(string tokenKey) => $"Auth:Yandex:{tokenKey}";

    private readonly IDataProtector _accessTokenProtector = dataProtectionProvider.CreateProtector("contest:oauth:access-token");
    private readonly IDataProtector _refreshTokenProtector = dataProtectionProvider.CreateProtector("contest:oauth:refresh-token");

    public Task StoreAccessTokenAsync(string key, TokenSet tokenSet)
    {
        var protectedAccessToken = _accessTokenProtector.Protect(tokenSet.AccessToken);

        var protectedRefreshToken = tokenSet.RefreshToken is not null
                                        ? _refreshTokenProtector.Protect(tokenSet.RefreshToken)
                                        : null;

        var protectedSet = tokenSet with { AccessToken = protectedAccessToken, RefreshToken = protectedRefreshToken };

        return cache.SetStringAsync(GetCacheKey(key), JsonSerializer.Serialize(protectedSet));
    }

    public async Task<TokenSet?> GetAccessTokenAsync(string key)
    {
        if (await cache.GetStringAsync(GetCacheKey(key)) is not { } payloadString)
            return null;

        try
        {
            var tokenSet = JsonSerializer.Deserialize<TokenSet>(payloadString);

            if (tokenSet is null)
                return null;

            var accessToken = _accessTokenProtector.Unprotect(tokenSet.AccessToken);
            var refreshToken = tokenSet.RefreshToken is not null
                                   ? _refreshTokenProtector.Unprotect(tokenSet.RefreshToken)
                                   : null;

            return tokenSet with { AccessToken = accessToken, RefreshToken = refreshToken };
        }
        catch
        {
            return null;
        }
    }
}
