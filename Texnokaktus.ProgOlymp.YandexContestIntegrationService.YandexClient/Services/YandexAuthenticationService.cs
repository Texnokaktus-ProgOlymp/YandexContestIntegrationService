using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RestSharp;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Options.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;

internal class YandexAuthenticationService([FromKeyedServices(ClientType.YandexOAuth)] IRestClient client,
                                           IOptions<YandexAppParameters> options) : IYandexAuthenticationService
{
    public string GetYandexOAuthUrl(string? localRedirectUri)
    {
        var request = new RestRequest("authorize").AddQueryParameter("client_id", options.Value.ClientId)
                                                  .AddQueryParameter("redirect_uri", localRedirectUri)
                                                  .AddQueryParameter("response_type", "code");

        return client.BuildUri(request).ToString();
    }

    public Task<TokenResponse> GetAccessTokenAsync(string code) =>
        RequestAccessTokenAsync(request => request.AddParameter("client_id", options.Value.ClientId)
                                                  .AddParameter("client_secret", options.Value.ClientSecret)
                                                  .AddParameter("grant_type", "authorization_code")
                                                  .AddParameter("code", code));

    public Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken) =>
        RequestAccessTokenAsync(request => request.AddParameter("client_id", options.Value.ClientId)
                                                  .AddParameter("client_secret", options.Value.ClientSecret)
                                                  .AddParameter("grant_type", "refresh_token")
                                                  .AddParameter("refresh_token", refreshToken));

    private async Task<TokenResponse> RequestAccessTokenAsync(Action<RestRequest> requestAction)
    {
        var request = new RestRequest("token");
        requestAction.Invoke(request);
        
        var response = await client.ExecutePostAsync<TokenResponse>(request);

        if (!response.IsSuccessful)
        {
            if (response.ErrorException is not null)
                throw new YandexAuthenticationException("An error occurred while requesting the access token", response.ErrorException);
            throw new YandexAuthenticationException("An error occurred while requesting the access token");
        }

        if (response.Data is null)
            throw new YandexAuthenticationException("Invalid data from OAuth server");

        return response.Data;
    }
}
