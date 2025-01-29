using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ParticipantService(IUnitOfWork unitOfWork) : IParticipantService
{
    public async Task<long?> GetContestUserIdAsync(int contestStageId, string participantLogin)
    {
        var contestUser = await unitOfWork.ContestUserRepository.GetAsync(contestStageId, participantLogin);
        return contestUser?.ContestUserId;
    }

    public async Task AddContestParticipantAsync(int contestStageId, string yandexIdLogin, long contestUserId)
    {
        unitOfWork.ContestUserRepository.Add(new(contestStageId, yandexIdLogin, contestUserId));
        await unitOfWork.SaveChangesAsync();
    }

    public Task DeleteContestParticipantAsync(int contestStageId, string yandexIdLogin) =>
        unitOfWork.ContestUserRepository.DeleteAsync(new(contestStageId, yandexIdLogin));
}
