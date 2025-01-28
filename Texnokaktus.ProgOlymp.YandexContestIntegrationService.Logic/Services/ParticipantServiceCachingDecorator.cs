using Microsoft.Extensions.Caching.Memory;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ParticipantServiceCachingDecorator(IParticipantService service, IMemoryCache cache) : IParticipantService
{
    public Task<long?> GetContestUserIdAsync(int contestStageId, string participantLogin) =>
        cache.GetOrCreateAsync($"Participant:{contestStageId}:{participantLogin}",
                               _ => service.GetContestUserIdAsync(contestStageId, participantLogin));
}
