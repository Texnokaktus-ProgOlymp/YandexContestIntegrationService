using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class RegistrationService(IContestStageService contestStageService,
                                   IParticipantService participantService,
                                   IContestClient contestClient) : IRegistrationService
{
    public async Task<string> RegisterUserAsync(int contestStageId, string yandexIdLogin)
    {
        if (await contestStageService.GetContestStageAsync(contestStageId) is not { } contestStage)
            throw new ContestStageDoesNotExistException(contestStageId);

        if (contestStage.YandexContestId is not { } yandexContestId)
            throw new YandexContestIdNotSetException(contestStageId);

        if (await participantService.GetContestUserIdAsync(contestStageId, yandexIdLogin) is not null)
            throw new UserIsAlreadyRegisteredException(contestStageId, yandexIdLogin);

        try
        {
            var contestUserId = await contestClient.RegisterParticipantByLoginAsync(yandexContestId, yandexIdLogin);

            await participantService.AddContestParticipantAsync(contestStageId, yandexIdLogin, contestUserId);

            return $"https://contest.yandex.ru/contest/{yandexContestId}/enter/";
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

        if (contestStage.YandexContestId is not { } yandexContestId)
            throw new YandexContestIdNotSetException(contestStageId);

        var contestUserId = await participantService.GetContestUserIdAsync(contestStageId, yandexIdLogin)
                         ?? throw new UserIsNotRegisteredException(contestStageId, yandexIdLogin);

        await contestClient.UnregisterParticipantAsync(yandexContestId, contestUserId);
        await participantService.DeleteContestParticipantAsync(contestStageId, yandexIdLogin);
    }
}
