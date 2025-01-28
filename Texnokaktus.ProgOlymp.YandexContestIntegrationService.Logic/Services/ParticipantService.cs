using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ParticipantService(IContestUserRepository contestUserRepository) : IParticipantService
{
    public async Task<long?> GetContestUserIdAsync(int contestStageId, string participantLogin)
    {
        var contestUser = await contestUserRepository.GetAsync(contestStageId, participantLogin);
        return contestUser?.ContestUserId;
    }
}
