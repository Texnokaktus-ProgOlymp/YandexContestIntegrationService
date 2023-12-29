using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services;

internal class UnitOfWork(AppDbContext context,
                          IContestStageApplicationRepository contestStageApplicationRepository,
                          IContestStageRepository contestStageRepository) : IUnitOfWork
{
    public IContestStageApplicationRepository ContestStageApplicationRepository { get; } = contestStageApplicationRepository;
    public IContestStageRepository ContestStageRepository { get; } = contestStageRepository;
    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
}
