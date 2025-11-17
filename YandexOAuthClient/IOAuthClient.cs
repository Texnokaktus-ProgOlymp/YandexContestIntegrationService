namespace YandexOAuthClient;

internal interface IOAuthClient
{
    Uri GetOAuthUri(string? localRedirectUri);
    Task<TokenResponse> GetAccessTokenAsync(string code);
    Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken);
}
