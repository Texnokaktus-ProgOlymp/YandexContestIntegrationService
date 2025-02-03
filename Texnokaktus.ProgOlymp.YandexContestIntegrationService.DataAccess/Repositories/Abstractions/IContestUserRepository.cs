using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;

public interface IContestUserRepository
{
    Task<bool> IsExistsAsync(long contestStageId, string yandexIdLogin);
    void Add(ContestUserInsertModel model);
    Task DeleteAsync(ContestUserDeleteModel model);
    Task<ContestUser?> GetAsync(long contestStageId, string yandexIdLogin);
    Task<IList<ContestUser>> GetAllAsync();
}
