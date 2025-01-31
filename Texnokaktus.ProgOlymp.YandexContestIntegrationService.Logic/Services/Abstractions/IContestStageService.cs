using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IContestStageService
{
    Task<IEnumerable<ContestStage>> GetContestStagesAsync();
    Task<ContestStage?> GetContestStageAsync(int id);
    Task AddContestStageAsync(ContestStage contestStage);
    Task SetYandexContestIdAsync(int contestStageId, long yandexContestId);
}
