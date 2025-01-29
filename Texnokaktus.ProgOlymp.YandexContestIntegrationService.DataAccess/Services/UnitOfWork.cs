using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services;

internal class UnitOfWork(AppDbContext context,
                          IContestStageRepository contestStageRepository,
                          IContestUserRepository contestUserRepository) : IUnitOfWork
{
    public IContestStageRepository ContestStageRepository { get; } = contestStageRepository;
    public IContestUserRepository ContestUserRepository { get; } = contestUserRepository;
    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
}
