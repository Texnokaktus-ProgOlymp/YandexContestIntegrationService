using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IApplicationService
{
    Task<IEnumerable<ContestStageApplication>> GetApplicationsAsync();
    Task<ContestStageApplication?> GetApplicationAsync(int id);
}
