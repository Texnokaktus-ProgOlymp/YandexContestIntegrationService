using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ParticipantService(IUnitOfWork unitOfWork, IContestClient contestClient) : IParticipantService
{
    public async Task<long?> GetContestUserIdAsync(long contestStageId, string participantLogin)
    {
        if (await unitOfWork.ContestUserRepository.GetAsync(contestStageId, participantLogin) is { } contestUser)
            return contestUser.ContestUserId;

        var participants = await contestClient.GetContestParticipantsAsync(contestStageId, null, participantLogin);

        if (participants.FirstOrDefault(x => x.Login == participantLogin) is not { } participantInfo)
            return null;

        await AddContestParticipantAsync(contestStageId, participantInfo.Login, participantInfo.Id);

        return participantInfo.Id;
    }

    public async Task AddContestParticipantAsync(long contestStageId, string yandexIdLogin, long contestUserId)
    {
        unitOfWork.ContestUserRepository.Add(new(contestStageId, yandexIdLogin, contestUserId));
        await unitOfWork.SaveChangesAsync();
    }

    public Task DeleteContestParticipantAsync(long contestStageId, string yandexIdLogin) =>
        unitOfWork.ContestUserRepository.DeleteAsync(new(contestStageId, yandexIdLogin));
}
