using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;

public interface IContestUserRepository
{
    Task<bool> IsExistsAsync(int contestStageId, string yandexIdLogin);
    void Add(ContestUserInsertModel model);
    Task DeleteAsync(ContestUserDeleteModel model);
    Task<ContestUser?> GetAsync(int contestStageId, string yandexIdLogin);
    Task<IList<ContestUser>> GetAllAsync();
}
