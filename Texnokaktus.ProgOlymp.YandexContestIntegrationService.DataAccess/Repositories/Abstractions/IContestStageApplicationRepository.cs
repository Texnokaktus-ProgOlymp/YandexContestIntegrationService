using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;

public interface IContestStageApplicationRepository
{
    Task<ContestStageApplication?> GetAsync(int id);
    Task<IList<ContestStageApplication>> GetAllAsync(ApplicationState? state);
    ContestStageApplication Add(ContestStageApplicationInsertModel insertModel);
    Task<ApplicationState?> GetStateAsync(int id);
    Task SetStateAsync(int id, ApplicationState state);
}
