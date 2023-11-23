using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services;

internal class UnitOfWork(AppDbContext context,
                          IContestStageApplicationRepository contestStageApplicationRepository) : IUnitOfWork
{
    public IContestStageApplicationRepository ContestStageApplicationRepository { get; } = contestStageApplicationRepository;
    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
}
