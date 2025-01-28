using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

public interface IContestClient
{
    Task<long> RegisterParticipantByLoginAsync(long contestId, string login);
    Task UnregisterParticipantAsync(long contestId, long participantId);
    Task UpdateParticipantAsync(long contestId, long participantId, UpdateParticipantRequest model);
    Task<ContestDescription> GetContestDescriptionAsync(long contestId);
    Task<ContestProblems> GetContestProblemsAsync(long contestId, string locale = "ru");

    Task<ContestStandings> GetContestStandingsAsync(long contestId,
                                                    bool forJudge = false,
                                                    string locale = "ru",
                                                    int page = 1,
                                                    int pageSize = 100,
                                                    string? participantSearch = null,
                                                    bool showExternal = false,
                                                    bool showVirtual = false,
                                                    long? userGroupId = null);

    Task<ParticipantStatus> GetParticipantStatusAsync(long contestId, long participantId);
}
