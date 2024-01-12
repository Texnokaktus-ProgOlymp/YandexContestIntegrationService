using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IApplicationService
{
    Task<IEnumerable<ContestStageApplication>> GetApplicationsAsync();
    Task<IEnumerable<ContestStageApplication>> GetPendingApplicationsAsync();
    Task<ContestStageApplication?> GetApplicationAsync(int id);
    Task AddApplicationAsync(int transactionId, int contestStageId, string yandexIdLogin);
}
