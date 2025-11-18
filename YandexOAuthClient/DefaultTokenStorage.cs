using System.Collections.Concurrent;

namespace YandexOAuthClient;

public class DefaultTokenStorage : ITokenStorage
{
    private static readonly ConcurrentDictionary<string, TokenSet> Storage = new();
    
    public Task StoreAccessTokenAsync(string key, TokenSet tokenSet)
    {
        Storage.AddOrUpdate(key, tokenSet, (_, _) => tokenSet);
        return Task.CompletedTask;
    }

    public Task<TokenSet?> GetAccessTokenAsync(string key) =>
        Storage.TryGetValue(key, out var value)
            ? Task.FromResult<TokenSet?>(value)
            : Task.FromResult<TokenSet?>(null);
}
