
namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IParticipantService
{
    Task<long?> GetContestUserIdAsync(long contestStageId, string participantLogin);
    Task AddContestParticipantAsync(long contestStageId, string yandexIdLogin, long contestUserId);
    Task DeleteContestParticipantAsync(long contestStageId, string yandexIdLogin);
}
