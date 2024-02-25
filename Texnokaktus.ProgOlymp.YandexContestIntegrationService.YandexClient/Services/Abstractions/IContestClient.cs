using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

public interface IContestClient
{
    Task<long> RegisterParticipantByLoginAsync(long contestId, string login);
    Task UnregisterParticipantAsync(long contestId, long participantId);
    Task<ContestProblems> GetContestProblemsAsync(long contestId, string locale = "ru");
}
