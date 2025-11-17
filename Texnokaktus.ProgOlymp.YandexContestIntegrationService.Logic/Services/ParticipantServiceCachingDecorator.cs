using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ParticipantServiceCachingDecorator(IParticipantService service, IMemoryCache cache) : IParticipantService
{
    public Task<long?> GetContestUserIdAsync(long contestStageId, int participantId) =>
        cache.GetOrCreateAsync(GetCacheKey(contestStageId, participantId),
                               _ => service.GetContestUserIdAsync(contestStageId, participantId));

    public Task<long?> GetContestUserIdAsync(long contestStageId, int participantId, string participantLogin) =>
        cache.GetOrCreateAsync($"Participant:{contestStageId}:{participantId}:{participantLogin}",
                               _ => service.GetContestUserIdAsync(contestStageId, participantId, participantLogin));

    public async Task AddContestParticipantAsync(long contestStageId, int participantId, string yandexIdLogin, long contestUserId)
    {
        await service.AddContestParticipantAsync(contestStageId, participantId, yandexIdLogin, contestUserId);
        cache.Remove(GetCacheKey(contestStageId, participantId));
    }

    public async Task DeleteContestParticipantAsync(long contestStageId, int participantId)
    {
        await service.DeleteContestParticipantAsync(contestStageId, participantId);
        cache.Remove(GetCacheKey(contestStageId, participantId));
    }

    private static string GetCacheKey(long contestStageId, int participantId) =>
        $"Participant:{contestStageId}:{participantId}";
}
