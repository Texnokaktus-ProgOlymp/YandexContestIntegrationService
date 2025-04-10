using System.Net;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;

internal class ContestClient([FromKeyedServices(ClientType.YandexContest)] IRestClient client) : IContestClient
{
    public async Task<long> RegisterParticipantByLoginAsync(long contestId, string login)
    {
        var request = new RestRequest("contests/{contestId}/participants").AddUrlSegment("contestId", contestId)
                                                                          .AddQueryParameter("login", login);
        var response = await client.ExecutePostAsync(request);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            if (response.ErrorException is not null)
                throw new InvalidUserException(response.Content, response.ErrorException);
            throw new InvalidUserException(response.Content);
        }

        response.ThrowIfError();

        return long.Parse(response.Content ?? throw new InvalidOperationException("Invalid API response"));
    }

    public Task UnregisterParticipantAsync(long contestId, long participantId) =>
        client.ExecuteAndThrowAsync("contests/{contestId}/participants/{participantId}",
                                    restRequest => restRequest.AddUrlSegment("contestId", contestId)
                                                              .AddUrlSegment("participantId", participantId),
                                    Method.Delete);

    public Task UpdateParticipantAsync(long contestId, long participantId, UpdateParticipantRequest model) =>
        client.ExecuteAndThrowAsync("contests/{contestId}/participants/{participantId}",
                                    restRequest => restRequest.AddUrlSegment("contestId", contestId)
                                                              .AddUrlSegment("participantId", participantId)
                                                              .AddJsonBody(model),
                                    Method.Patch);

    public Task<ParticipantInfo[]> GetContestParticipantsAsync(long contestId, string? displayName, string? login) =>
        client.ExecuteGetAndThrowAsync<ParticipantInfo[]>("contests/{contestId}/participants",
                                                          request => request.AddUrlSegment("contestId", contestId)
                                                                            .AddQueryParameterIfNotNull("display_name", displayName)
                                                                            .AddQueryParameterIfNotNull("login", login));
}

file static class ApiClientExtensions
{
    public static async Task ExecuteAndThrowAsync(this IRestClient client,
                                                  string urlPath,
                                                  Func<RestRequest, RestRequest> requestAction,
                                                  Method method)
    {
        var request = requestAction.Invoke(new(urlPath));
        await client.ExecuteAndThrowAsync(request, method);
    }
    
    public static async Task<TResult> ExecuteGetAndThrowAsync<TResult>(this IRestClient client,
                                                                       string urlPath,
                                                                       Func<RestRequest, RestRequest> requestAction)
    {
        var request = requestAction.Invoke(new(urlPath));
        return await client.ExecuteGetAndThrowAsync<TResult>(request);
    }


    private static async Task ExecuteAndThrowAsync(this IRestClient client, RestRequest request, Method method)
    {
        var response = await client.ExecuteAsync(request, method);

        if (!response.IsSuccessful)
        {
            if (response.ErrorException is not null)
                throw new YandexApiException("An error occurred while requesting the API", response.ErrorException);
            throw new YandexApiException("An error occurred while requesting the API");
        }

        response.ThrowIfError();
    }

    private static async Task<TResult> ExecuteGetAndThrowAsync<TResult>(this IRestClient client, RestRequest request)
    {
        var response = await client.ExecuteGetAsync<TResult>(request);

        if (!response.IsSuccessful)
        {
            if (response.ErrorException is not null)
                throw new YandexApiException("An error occurred while requesting the API", response.ErrorException);
            throw new YandexApiException("An error occurred while requesting the API");
        }

        if (response.Data is null)
            throw new YandexApiException("Invalid data from API");

        return response.Data;
    }

    public static RestRequest AddQueryParameterIfNotNull(this RestRequest request,
                                                         string name,
                                                         string? value,
                                                         bool encode = true) =>
        value is not null
            ? request.AddQueryParameter(name, value, encode)
            : request;
}
