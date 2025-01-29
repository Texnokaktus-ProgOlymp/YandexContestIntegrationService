using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories;

internal class ContestUserRepository(AppDbContext context) : IContestUserRepository
{
    public async Task<bool> IsExistsAsync(int contestStageId, string yandexIdLogin) =>
        await context.ContestUsers
                     .AnyAsync(contestUser => contestUser.ContestStageId == contestStageId
                                           && contestUser.YandexIdLogin == yandexIdLogin);

    public void Add(ContestUserInsertModel model)
    {
        var entity = new ContestUser
        {
            ContestStageId = model.ContestStageId,
            YandexIdLogin = model.YandexIdLogin,
            ContestUserId = model.ContestUserId
        };
        context.ContestUsers.Add(entity);
    }

    public async Task DeleteAsync(ContestUserDeleteModel model) =>
        await context.ContestUsers
                     .Where(contestUser => contestUser.ContestStageId == model.ContestStageId
                                        && contestUser.YandexIdLogin == model.YandexIdLogin)
                     .ExecuteDeleteAsync();

    public async Task<ContestUser?> GetAsync(int contestStageId, string yandexIdLogin) =>
        await context.ContestUsers
                     .AsNoTracking()
                     .Include(contestUser => contestUser.ContestStage)
                     .Where(user => user.ContestStageId == contestStageId
                                 && user.YandexIdLogin == yandexIdLogin)
                     .FirstOrDefaultAsync();

    public async Task<IList<ContestUser>> GetAllAsync() =>
        await context.ContestUsers
                     .AsNoTracking()
                     .Include(contestUser => contestUser.ContestStage)
                     .ToListAsync();
}
