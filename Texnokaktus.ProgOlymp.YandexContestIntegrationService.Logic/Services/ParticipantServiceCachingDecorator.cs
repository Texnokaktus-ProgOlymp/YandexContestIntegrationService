using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ParticipantServiceCachingDecorator(IParticipantService service, IMemoryCache cache) : IParticipantService
{
    public Task<long?> GetContestUserIdAsync(int contestStageId, string participantLogin) =>
        cache.GetOrCreateAsync(GetCacheKey(contestStageId, participantLogin),
                               _ => service.GetContestUserIdAsync(contestStageId, participantLogin));

    public async Task AddContestParticipantAsync(int contestStageId, string yandexIdLogin, long contestUserId)
    {
        await service.AddContestParticipantAsync(contestStageId, yandexIdLogin, contestUserId);
        cache.Remove(GetCacheKey(contestStageId, yandexIdLogin));
    }

    public async Task DeleteContestParticipantAsync(int contestStageId, string yandexIdLogin)
    {
        await service.DeleteContestParticipantAsync(contestStageId, yandexIdLogin);
        cache.Remove(GetCacheKey(contestStageId, yandexIdLogin));
    }

    private static string GetCacheKey(int contestStageId, string participantLogin) =>
        $"Participant:{contestStageId}:{participantLogin}";
}
