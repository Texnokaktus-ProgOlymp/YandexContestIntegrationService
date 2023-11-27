using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Mapping;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ApplicationService(IContestStageApplicationRepository repository) : IApplicationService
{
    public async Task<IEnumerable<ContestStageApplication>> GetApplicationsAsync()
    {
        var applications = await repository.GetAllAsync(null);
        return applications.Select(a => a.MapContestStageApplication());
    }

    public async Task<ContestStageApplication?> GetApplicationAsync(int id)
    {
        var application = await repository.GetAsync(id);
        return application?.MapContestStageApplication();
    }
}
