using System.Net;
using RestSharp;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;
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
}
