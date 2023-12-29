using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;

public interface IUnitOfWork
{
    IContestStageApplicationRepository ContestStageApplicationRepository { get; }
    IContestStageRepository ContestStageRepository { get; }
    Task SaveChangesAsync();
}
