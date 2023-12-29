using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;

public interface IContestStageRepository
{
    Task<ContestStage?> GetAsync(int id);
    Task<IList<ContestStage>> GetAllAsync();
    void Add(int contestStageId, long yandexContestId);
}
