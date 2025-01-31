using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories;

internal class ContestStageRepository(AppDbContext context) : IContestStageRepository
{
    public async Task<ContestStage?> GetAsync(int id) =>
        await context.ContestStages.FirstOrDefaultAsync(stage => stage.Id == id);

    public async Task<IList<ContestStage>> GetAllAsync() =>
        await context.ContestStages.ToListAsync();

    public void Add(int contestStageId, long yandexContestId)
    {
        var entity = new ContestStage
        {
            Id = contestStageId,
            YandexContestId = yandexContestId
        };
        context.ContestStages.Add(entity);
    }

    public async ValueTask UpdateAsync(int id, Action<ContestStage> updateAction)
    {
        var contestStage = await context.ContestStages.FindAsync(id)
                        ?? throw new EntityNotFoundException<ContestStage>($"Could not find the {nameof(ContestStage)} with ID {id}.");
        updateAction.Invoke(contestStage);
    }
}
