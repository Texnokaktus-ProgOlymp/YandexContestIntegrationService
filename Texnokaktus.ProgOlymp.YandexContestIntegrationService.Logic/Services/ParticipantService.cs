using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using YandexContestClient.Client;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ParticipantService(IUnitOfWork unitOfWork, ContestClient contestClient) : IParticipantService
{
    public async Task<long?> GetContestUserIdAsync(long contestStageId, string participantLogin)
    {
        if (await unitOfWork.ContestUserRepository.GetAsync(contestStageId, participantLogin) is { } contestUser)
            return contestUser.ContestUserId;

        var participants = await contestClient.Contests[contestStageId]
                                              .Participants
                                              .GetAsync(configuration => configuration.QueryParameters.Login = participantLogin);

        if (participants?.FirstOrDefault(x => x.Login == participantLogin) is not { Id: not null, Login: not null } participantInfo)
            return null;

        await AddContestParticipantAsync(contestStageId, participantInfo.Login, participantInfo.Id.Value);

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
