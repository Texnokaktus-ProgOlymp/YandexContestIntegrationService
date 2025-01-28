using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ContestStageServiceCachingDecorator(IContestStageService service, IMemoryCache cache) : IContestStageService
{
    public Task<IEnumerable<ContestStage>> GetContestStagesAsync() => service.GetContestStagesAsync();

    public Task<ContestStage?> GetContestStageAsync(int id) =>
        cache.GetOrCreateAsync(GetCacheKey(id), _ => service.GetContestStageAsync(id));

    public async Task AddContestStageAsync(ContestStage contestStage)
    {
        await service.AddContestStageAsync(contestStage);
        cache.Remove(GetCacheKey(contestStage.Id));
    }

    public async Task AddContestStageAsync(int contestStageId)
    {
        await service.AddContestStageAsync(contestStageId);
        cache.Remove(GetCacheKey(contestStageId));
    }

    public async Task SetYandexContestIdAsync(int contestStageId, long yandexContestId)
    {
        await service.SetYandexContestIdAsync(contestStageId, yandexContestId);
        cache.Remove(GetCacheKey(contestStageId));
    }

    private static string GetCacheKey(int contestStageId) => $"ContestStage:{contestStageId}";
}
