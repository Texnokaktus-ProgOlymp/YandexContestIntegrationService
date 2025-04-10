using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;

internal class YandexOAuthAuthenticationProvider(ITokenService tokenService) : IAuthenticationProvider
{
    private const string AuthorizationHeaderKey = "Authorization";
    private const string AuthorizationHeaderValuePrefix = "OAuth";

    public async Task AuthenticateRequestAsync(RequestInformation request,
                                               Dictionary<string, object>? additionalAuthenticationContext = null,
                                               CancellationToken cancellationToken = new())
    {

        ArgumentNullException.ThrowIfNull(request);

        request.Headers.Remove(AuthorizationHeaderKey);

        if (!request.Headers.ContainsKey(AuthorizationHeaderKey))
        {
            var accessToken = await tokenService.GetAccessTokenAsync() ?? throw new("No token");
            request.Headers.Add(AuthorizationHeaderKey, $"{AuthorizationHeaderValuePrefix} {accessToken}");
        }
    }
}
