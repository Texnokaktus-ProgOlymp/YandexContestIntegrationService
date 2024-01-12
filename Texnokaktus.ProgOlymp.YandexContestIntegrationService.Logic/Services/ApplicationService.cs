using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Mapping;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using ApplicationState = Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities.ApplicationState;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ApplicationService(IUnitOfWork unitOfWork, TimeProvider timeProvider) : IApplicationService
{
    public async Task<IEnumerable<ContestStageApplication>> GetApplicationsAsync()
    {
        var applications = await unitOfWork.ContestStageApplicationRepository.GetAllAsync(null);
        return applications.Select(a => a.MapContestStageApplication());
    }

    public async Task<ContestStageApplication?> GetApplicationAsync(int id)
    {
        var application = await unitOfWork.ContestStageApplicationRepository.GetAsync(id);
        return application?.MapContestStageApplication();
    }

    public async Task AddApplicationAsync(int transactionId, int contestStageId, string yandexIdLogin)
    {
        var insertModel = new ContestStageApplicationInsertModel(transactionId,
                                                                 yandexIdLogin,
                                                                 contestStageId,
                                                                 ApplicationState.Pending,
                                                                 timeProvider.GetUtcNow().DateTime);
        unitOfWork.ContestStageApplicationRepository.Add(insertModel);
        await unitOfWork.SaveChangesAsync();
    }
}
