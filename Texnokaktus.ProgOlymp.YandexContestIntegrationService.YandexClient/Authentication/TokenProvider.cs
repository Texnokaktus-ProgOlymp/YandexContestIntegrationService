using Microsoft.Kiota.Abstractions.Authentication;
using YandexOAuthClient.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Authentication;

public class TokenProvider(IAuthService authService) : IAccessTokenProvider
{
    public async Task<string> GetAuthorizationTokenAsync(Uri uri,
                                                         Dictionary<string, object>? additionalAuthenticationContext = null,
                                                         CancellationToken cancellationToken = default) =>
        AllowedHostsValidator.IsUrlHostValid(uri)
            ? await authService.GetAccessTokenAsync("DEFAULT") ?? throw new("No access token")
            : string.Empty;

    public AllowedHostsValidator AllowedHostsValidator => new(["api.contest.yandex.net"]);
}
