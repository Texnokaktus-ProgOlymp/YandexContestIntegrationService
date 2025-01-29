
namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IParticipantService
{
    Task<long?> GetContestUserIdAsync(int contestStageId, string participantLogin);
    Task AddContestParticipantAsync(int contestStageId, string yandexIdLogin, long contestUserId);
    Task DeleteContestParticipantAsync(int contestStageId, string yandexIdLogin);
}
