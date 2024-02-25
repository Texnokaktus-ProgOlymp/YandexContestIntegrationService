using System.Net;
using RestSharp;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services;

internal class ContestClient(IRestClient client) : IContestClient
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

    public async Task UnregisterParticipantAsync(long contestId, long participantId)
    {
        var request = new RestRequest("contests/{contestId}/participants/{participantId}").AddUrlSegment("contestId", contestId)
                                                                                          .AddUrlSegment("participantId", participantId);
        var response = await client.ExecuteAsync(request, Method.Delete);
        response.ThrowIfError();
    }

    public async Task<ContestProblems> GetContestProblemsAsync(long contestId, string locale = "ru") =>
        await client.ExecuteGetAndThrowAsync<ContestProblems>("contests/{contestId}/problems",
                                                              request => request.AddUrlSegment("contestId", contestId)
                                                                                .AddQueryParameter("locale", locale));
}

file static class ApiClientExtensions
{
    public static async Task<TResult> ExecuteGetAndThrowAsync<TResult>(this IRestClient client,
                                                                       string urlPath,
                                                                       Func<RestRequest, RestRequest> requestAction)
    {
        var request = requestAction.Invoke(new(urlPath));
        return await client.ExecuteGetAndThrowAsync<TResult>(request);
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
}
