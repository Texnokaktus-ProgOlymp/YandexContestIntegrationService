using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;

public interface IUnitOfWork
{
    IContestUserRepository ContestUserRepository { get; }
    Task SaveChangesAsync();
}
