using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using YandexOAuthClient;
using YandexOAuthClient.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;

internal class CacheTokenStorage(IDistributedCache cache) : ITokenStorage
{
    private static string GetCacheKey(string tokenKey) => $"Auth:Yandex:{tokenKey}";

    public Task StoreAccessTokenAsync(string key, TokenSet tokenSet) =>
        cache.SetStringAsync(GetCacheKey(key), JsonSerializer.Serialize(tokenSet));

    public async Task<TokenSet?> GetAccessTokenAsync(string key)
    {
        if (await cache.GetStringAsync(GetCacheKey(key)) is not { } payloadString)
            return null;

        try
        {
            return JsonSerializer.Deserialize<TokenSet>(payloadString);
        }
        catch
        {
            return null;
        }
    }
}
