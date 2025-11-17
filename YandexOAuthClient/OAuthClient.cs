using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Web;
using Microsoft.Extensions.Options;

namespace YandexOAuthClient;

internal class OAuthClient(HttpClient httpClient, IOptions<YandexAppParameters> options) : IOAuthClient
{
    private UriBuilder UriBuilder => httpClient.BaseAddress is { } baseAddress
                                         ? new(baseAddress)
                                         : new();

    public Uri GetOAuthUri(string? localRedirectUri)
    {
        var uriBuilder = UriBuilder;
        uriBuilder.Path = "authorize";
        uriBuilder.Query = GetQueryString(GetOAuthUriQueryParameters(localRedirectUri));

        return uriBuilder.Uri;
    }

    public Task<TokenResponse> GetAccessTokenAsync(string code) =>
        RequestAccessTokenAsync(GetAccessTokenQueryParameters(code));

    public Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken) =>
        RequestAccessTokenAsync(RefreshAccessTokenQueryParameters(refreshToken));

    private async Task<TokenResponse> RequestAccessTokenAsync(IEnumerable<KeyValuePair<string, string?>> parameters)
    {
        var uriBuilder = UriBuilder;
        uriBuilder.Path = "token";
        uriBuilder.Query = GetQueryString(parameters);

        var responseMessage = await httpClient.PostAsync(uriBuilder.Uri, null);
        return await responseMessage.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<TokenResponse>()
            ?? throw new();
    }

    private static string GetQueryString(IEnumerable<KeyValuePair<string, string?>> parameters)
    {
        var queryCollection = HttpUtility.ParseQueryString(string.Empty);
        foreach (var (parameterName, parameterValue) in parameters)
            AddParameterNotNull(queryCollection, parameterName, parameterValue);
        return queryCollection.ToString() ?? string.Empty;
    }

    private static void AddParameterNotNull(NameValueCollection collection, string parameterName, string? parameterValue)
    {
        if (parameterValue is null) return;
        collection[parameterName] = parameterValue;
    }

    private IEnumerable<KeyValuePair<string, string?>> GetOAuthUriQueryParameters(string? redirectUri)
    {
        yield return KeyValuePair.Create<string, string?>("client_id", options.Value.ClientId);
        yield return KeyValuePair.Create<string, string?>("redirect_uri", redirectUri);
        yield return KeyValuePair.Create<string, string?>("response_type", "code");
    }

    private IEnumerable<KeyValuePair<string, string?>> GetAccessTokenQueryParameters(string code)
    {
        yield return KeyValuePair.Create<string, string?>("client_id", options.Value.ClientId);
        yield return KeyValuePair.Create<string, string?>("client_secret", options.Value.ClientSecret);
        yield return KeyValuePair.Create<string, string?>("grant_type", "authorization_code");
        yield return KeyValuePair.Create<string, string?>("code", code);
    }

    private IEnumerable<KeyValuePair<string, string?>> RefreshAccessTokenQueryParameters(string refreshToken)
    {
        yield return KeyValuePair.Create<string, string?>("client_id", options.Value.ClientId);
        yield return KeyValuePair.Create<string, string?>("client_secret", options.Value.ClientSecret);
        yield return KeyValuePair.Create<string, string?>("grant_type", "refresh_token");
        yield return KeyValuePair.Create<string, string?>("refresh_token", refreshToken);
    }
}
