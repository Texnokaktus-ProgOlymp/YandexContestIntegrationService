namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

public interface IContestClient
{
    Task<long> RegisterParticipantByLogin(long contestId, string login);
    Task UnregisterParticipant(long contestId, long participantId);
}
