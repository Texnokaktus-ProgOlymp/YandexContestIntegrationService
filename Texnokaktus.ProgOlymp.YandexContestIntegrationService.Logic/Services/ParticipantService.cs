using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using YandexContestClient.Client;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ParticipantService(AppDbContext context, ContestClient contestClient) : IParticipantService
{
    public async Task<long?> GetContestUserIdAsync(long contestStageId, int participantId) =>
        await context.ContestUsers
                     .Where(user => user.ContestStageId == contestStageId
                                 && user.ParticipantId == participantId)
                     .Select<ContestUser, long?>(x => x.ContestUserId)
                     .FirstOrDefaultAsync();

    public async Task AddContestParticipantAsync(long contestStageId, int participantId, string yandexIdLogin, long contestUserId)
    {
        context.ContestUsers.Add(new()
        {
            ParticipantId = participantId,
            ContestStageId = contestStageId,
            YandexIdLogin = yandexIdLogin,
            ContestUserId = contestUserId
        });
        await context.SaveChangesAsync();
    }

    public Task DeleteContestParticipantAsync(long contestStageId, int participantId) =>
        context.ContestUsers
               .Where(contestUser => contestUser.ContestStageId == contestStageId
                                  && contestUser.ParticipantId == participantId)
               .ExecuteDeleteAsync();
}
