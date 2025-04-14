using Microsoft.Kiota.Abstractions.Authentication;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Authentication;

public class TokenProvider(ITokenService tokenService) : IAccessTokenProvider
{
    public async Task<string> GetAuthorizationTokenAsync(Uri uri,
                                                         Dictionary<string, object>? additionalAuthenticationContext = null,
                                                         CancellationToken cancellationToken = default) =>
        AllowedHostsValidator.IsUrlHostValid(uri)
            ? await tokenService.GetAccessTokenAsync() ?? throw new("No access token")
            : string.Empty;

    public AllowedHostsValidator AllowedHostsValidator => new(["api.contest.yandex.net"]);
}
