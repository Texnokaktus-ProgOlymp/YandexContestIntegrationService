using Microsoft.Extensions.Logging;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ContestStageService(IUnitOfWork unitOfWork, ILogger<ContestStageService> logger) : IContestStageService
{
    public async Task<IEnumerable<ContestStage>> GetContestStagesAsync()
    {
        var contestStages = await unitOfWork.ContestStageRepository.GetAllAsync();
        return contestStages.Select(stage => stage.MapContestStage());
    }

    public async Task<ContestStage?> GetContestStageAsync(int id)
    {
        var contestStage = await unitOfWork.ContestStageRepository.GetAsync(id);
        return contestStage?.MapContestStage();
    }

    public async Task AddContestStageAsync(ContestStage contestStage)
    {
        unitOfWork.ContestStageRepository.Add(contestStage.Id, contestStage.YandexContestId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddContestStageAsync(int contestStageId)
    {
        unitOfWork.ContestStageRepository.Add(contestStageId, null);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Added the new Contest Stage with ID {ContestStageId}. "
                            + $"Note that you must set the {nameof(ContestStage.YandexContestId)} value for this Contest Stage manually",
                              contestStageId);
    }

    public async Task SetYandexContestIdAsync(int contestStageId, long yandexContestId)
    {
        await unitOfWork.ContestStageRepository.UpdateAsync(contestStageId, stage => stage.YandexContestId ??= yandexContestId);
        await unitOfWork.SaveChangesAsync();
    }
}

file static class MappingExtensions
{
    public static ContestStage MapContestStage(this DataAccess.Entities.ContestStage contestStage) =>
        new(contestStage.Id, contestStage.YandexContestId);
}
