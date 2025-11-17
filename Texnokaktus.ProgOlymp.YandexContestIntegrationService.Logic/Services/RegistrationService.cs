using Microsoft.Extensions.Logging;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;
using YandexContestClient.Client;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class RegistrationService(IParticipantService participantService,
                                   ContestClient contestClient,
                                   ILogger<RegistrationService> logger) : IRegistrationService
{
    public async Task RegisterUserAsync(long contestStageId, string yandexIdLogin, string? participantDisplayName, int participantId)
    {
        try
        {
            var contestUserId = await contestClient.Contests[contestStageId]
                                                   .Participants
                                                   .PostAsync(configuration => configuration.QueryParameters.Login = yandexIdLogin)
                             ?? throw new YandexApiException("Unable to get User Id");

            if (participantDisplayName is not null)
                await SetParticipantDisplayNameAsync(contestStageId, contestUserId, participantDisplayName);

            if (await participantService.GetContestUserIdAsync(contestStageId, participantId, yandexIdLogin) is null)
                await participantService.AddContestParticipantAsync(contestStageId, participantId, yandexIdLogin, contestUserId);
        }
        catch (InvalidUserException e)
        {
            throw new InvalidYandexUserException(e);
        }
    }

    public async Task UnregisterUserAsync(long contestStageId, int participantId)
    {
        var contestUserId = await participantService.GetContestUserIdAsync(contestStageId, participantId)
                         ?? throw new UserIsNotRegisteredException(contestStageId, participantId);

        await contestClient.Contests[contestStageId].Participants[contestUserId].DeleteAsync();
        await participantService.DeleteContestParticipantAsync(contestStageId, participantId);
    }

    private async Task SetParticipantDisplayNameAsync(long yandexContestId, long yandexParticipantId, string displayName)
    {
        try
        {
            await contestClient.Contests[yandexContestId]
                               .Participants[yandexParticipantId]
                               .PatchAsync(new()
                                {
                                    DisplayedName = displayName
                                });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to update participant's display name");
        }
    }
}
