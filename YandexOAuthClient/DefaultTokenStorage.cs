using System.Collections.Concurrent;

namespace YandexOAuthClient;

public class DefaultTokenStorage : ITokenStorage
{
    private readonly ConcurrentDictionary<string, TokenResponse> _storage = new();
    
    public Task StoreAccessTokenAsync(string key, TokenResponse tokenResponse)
    {
        _storage.AddOrUpdate(key, tokenResponse, (_, _) => tokenResponse);
        return Task.CompletedTask;
    }

    public Task<TokenResponse?> GetAccessTokenAsync(string key) =>
        _storage.TryGetValue(key, out var value)
            ? Task.FromResult<TokenResponse?>(value)
            : Task.FromResult<TokenResponse?>(null);
}
