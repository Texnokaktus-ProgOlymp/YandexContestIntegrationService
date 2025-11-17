namespace YandexOAuthClient;

public interface ITokenStorage
{
    Task StoreAccessTokenAsync(string key, TokenResponse tokenResponse);
    Task<TokenResponse?> GetAccessTokenAsync(string key);
}
