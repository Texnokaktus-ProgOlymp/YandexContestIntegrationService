
namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IParticipantService
{
    Task<long?> GetContestUserIdAsync(int contestStageId, string participantLogin);
}
