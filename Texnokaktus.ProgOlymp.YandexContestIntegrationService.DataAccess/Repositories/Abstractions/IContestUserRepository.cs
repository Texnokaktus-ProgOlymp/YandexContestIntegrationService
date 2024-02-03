using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;

public interface IContestUserRepository
{
    public Task<bool> IsExistsAsync(int contestStageId, string yandexIdLogin);
    public void Add(ContestUserInsertModel model);
    public Task DeleteAsync(ContestUserDeleteModel model);
}
