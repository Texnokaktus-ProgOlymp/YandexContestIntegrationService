using Microsoft.AspNetCore.DataProtection;
using YandexOAuthClient;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;

internal class EncryptedStorageDecorator(ITokenStorage tokenStorage, IDataProtectionProvider dataProtectionProvider) : ITokenStorage
{
    private readonly IDataProtector _accessTokenProtector = dataProtectionProvider.CreateProtector("contest:oauth:access-token");
    private readonly IDataProtector _refreshTokenProtector = dataProtectionProvider.CreateProtector("contest:oauth:refresh-token");

    public Task StoreAccessTokenAsync(string key, TokenSet tokenSet)
    {
        var protectedAccessToken = _accessTokenProtector.Protect(tokenSet.AccessToken);

        var protectedRefreshToken = tokenSet.RefreshToken is not null
                                        ? _refreshTokenProtector.Protect(tokenSet.RefreshToken)
                                        : null;

        var protectedSet = tokenSet with { AccessToken = protectedAccessToken, RefreshToken = protectedRefreshToken };

        return tokenStorage.StoreAccessTokenAsync(key, protectedSet);
    }

    public async Task<TokenSet?> GetAccessTokenAsync(string key)
    {
        if (await tokenStorage.GetAccessTokenAsync(key) is not { } tokenSet)
            return null;

        try
        {
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
