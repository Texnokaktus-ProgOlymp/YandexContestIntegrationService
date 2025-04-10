using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

public interface IContestClient
{
    Task<long> RegisterParticipantByLoginAsync(long contestId, string login);
    Task UnregisterParticipantAsync(long contestId, long participantId);
    Task UpdateParticipantAsync(long contestId, long participantId, UpdateParticipantRequest model);
    Task<ParticipantInfo[]> GetContestParticipantsAsync(long contestId, string? displayName, string? login);
}
