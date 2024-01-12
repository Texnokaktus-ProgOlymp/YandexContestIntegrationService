using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Extensions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories;

internal class ContestStageApplicationRepository(AppDbContext context) : IContestStageApplicationRepository
{
    public async Task<ContestStageApplication?> GetAsync(int id) =>
        await context.ContestStageApplications.FirstOrDefaultAsync(application => application.Id == id);

    public async Task<IList<ContestStageApplication>> GetAllAsync(ApplicationState? state) =>
        await context.ContestStageApplications
                     .WhereIf(state.HasValue, application => application.State == state!.Value)
                     .Include(application => application.ContestStage)
                     .ToListAsync();

    public ContestStageApplication Add(ContestStageApplicationInsertModel insertModel)
    {
        var application = new ContestStageApplication
        {
            TransactionId = insertModel.TransactionId,
            YandexIdLogin = insertModel.YandexIdLogin,
            ContestStageId = insertModel.ContestStageId,
            State = insertModel.State,
            CreatedUtc = insertModel.CreatedUtc
        };
        var entityEntry = context.ContestStageApplications.Add(application);
        return entityEntry.Entity;
    }

    public async Task<ApplicationState?> GetStateAsync(int id) =>
        await context.ContestStageApplications
                     .Where(application => application.ContestStageId == id)
                     .Select(application => application.State)
                     .FirstOrDefaultAsync();

    public async Task SetStateAsync(int id, ApplicationState state)
    {
        var application = await GetAsync(id);
        if (application is null) return;
        application.State = state;
        context.ContestStageApplications.Update(application);
    }
}
