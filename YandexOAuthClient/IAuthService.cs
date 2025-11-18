namespace YandexOAuthClient;

public interface IAuthService
{
    string GetOAuthUrl(string? redirectUrl);
    Task AuthorizeAsync(string key, string authCode);
    Task<string?> GetAccessTokenAsync(string key);
}
