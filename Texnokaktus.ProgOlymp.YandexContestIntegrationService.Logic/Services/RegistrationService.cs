using Microsoft.Extensions.Logging;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class RegistrationService(IContestStageService contestStageService,
                                   IParticipantService participantService,
                                   IContestClient contestClient,
                                   ILogger<RegistrationService> logger) : IRegistrationService
{
    public async Task RegisterUserAsync(int contestStageId, string yandexIdLogin, string? participantDisplayName)
    {
        if (await contestStageService.GetContestStageAsync(contestStageId) is not { } contestStage)
            throw new ContestStageDoesNotExistException(contestStageId);

        if (await participantService.GetContestUserIdAsync(contestStageId, yandexIdLogin) is not null)
            throw new UserIsAlreadyRegisteredException(contestStageId, yandexIdLogin);

        try
        {
            var contestUserId = await contestClient.RegisterParticipantByLoginAsync(contestStage.YandexContestId, yandexIdLogin);
            if (participantDisplayName is not null)
                await SetParticipantDisplayNameAsync(contestStage.YandexContestId, contestUserId, participantDisplayName);

            await participantService.AddContestParticipantAsync(contestStageId, yandexIdLogin, contestUserId);
        }
        catch (InvalidUserException e)
        {
            throw new InvalidYandexUserException(e);
        }
    }

    public async Task UnregisterUserAsync(int contestStageId, string yandexIdLogin)
    {
        if (await contestStageService.GetContestStageAsync(contestStageId) is not { } contestStage)
            throw new ContestStageDoesNotExistException(contestStageId);

        var contestUserId = await participantService.GetContestUserIdAsync(contestStageId, yandexIdLogin)
                         ?? throw new UserIsNotRegisteredException(contestStageId, yandexIdLogin);

        await contestClient.UnregisterParticipantAsync(contestStage.YandexContestId, contestUserId);
        await participantService.DeleteContestParticipantAsync(contestStageId, yandexIdLogin);
    }

    private async Task SetParticipantDisplayNameAsync(long yandexContestId, long yandexParticipantId, string displayName)
    {
        try
        {
            await contestClient.UpdateParticipantAsync(yandexContestId, yandexParticipantId, new(displayName));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to update participant's display name");
        }
    }
}
