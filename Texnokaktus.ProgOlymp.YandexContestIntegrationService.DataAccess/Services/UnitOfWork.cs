using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services;

internal class UnitOfWork(AppDbContext context,
                          IContestUserRepository contestUserRepository) : IUnitOfWork
{
    public IContestUserRepository ContestUserRepository { get; } = contestUserRepository;
    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
}
