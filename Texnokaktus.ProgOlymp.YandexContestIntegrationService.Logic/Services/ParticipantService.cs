using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using YandexContestClient.Client;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ParticipantService(IUnitOfWork unitOfWork, ContestClient contestClient) : IParticipantService
{
    public async Task<long?> GetContestUserIdAsync(long contestStageId, int participantId)
    {
        if (await unitOfWork.ContestUserRepository.GetAsync(contestStageId, participantId) is { } contestUser)
            return contestUser.ContestUserId;

        return null;
    }

    public async Task<long?> GetContestUserIdAsync(long contestStageId, int participantId, string participantLogin)
    {
        if (await GetContestUserIdAsync(contestStageId, participantId) is { } contestUserId)
            return contestUserId;

        var participants = await contestClient.Contests[contestStageId]
                                              .Participants
                                              .GetAsync(configuration => configuration.QueryParameters.Login = participantLogin);

        if (participants?.FirstOrDefault(x => x.Login == participantLogin) is not { Id: not null, Login: not null } participantInfo)
            return null;

        await AddContestParticipantAsync(contestStageId, participantId, participantInfo.Login, participantInfo.Id.Value);

        return participantInfo.Id;
    }

    public async Task AddContestParticipantAsync(long contestStageId, int participantId, string yandexIdLogin, long contestUserId)
    {
        unitOfWork.ContestUserRepository.Add(new(participantId, contestStageId, yandexIdLogin, contestUserId));
        await unitOfWork.SaveChangesAsync();
    }

    public Task DeleteContestParticipantAsync(long contestStageId, int participantId) =>
        unitOfWork.ContestUserRepository.DeleteAsync(new(contestStageId, participantId));
}
