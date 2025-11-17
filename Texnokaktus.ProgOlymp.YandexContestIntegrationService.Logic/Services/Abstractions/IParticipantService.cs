namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IParticipantService
{
    Task<long?> GetContestUserIdAsync(long contestStageId, int participantId);
    Task<long?> GetContestUserIdAsync(long contestStageId, int participantId, string participantLogin);
    Task AddContestParticipantAsync(long contestStageId, int participantId, string yandexIdLogin, long contestUserId);
    Task DeleteContestParticipantAsync(long contestStageId, int participantId);
}
