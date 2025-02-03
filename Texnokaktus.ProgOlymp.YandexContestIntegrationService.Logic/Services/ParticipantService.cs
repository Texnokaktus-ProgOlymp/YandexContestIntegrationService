using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ParticipantService(IUnitOfWork unitOfWork) : IParticipantService
{
    public async Task<long?> GetContestUserIdAsync(long contestStageId, string participantLogin)
    {
        var contestUser = await unitOfWork.ContestUserRepository.GetAsync(contestStageId, participantLogin);
        return contestUser?.ContestUserId;
    }

    public async Task AddContestParticipantAsync(long contestStageId, string yandexIdLogin, long contestUserId)
    {
        unitOfWork.ContestUserRepository.Add(new(contestStageId, yandexIdLogin, contestUserId));
        await unitOfWork.SaveChangesAsync();
    }

    public Task DeleteContestParticipantAsync(long contestStageId, string yandexIdLogin) =>
        unitOfWork.ContestUserRepository.DeleteAsync(new(contestStageId, yandexIdLogin));
}
