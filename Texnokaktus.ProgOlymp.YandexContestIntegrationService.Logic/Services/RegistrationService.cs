using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class RegistrationService(IUnitOfWork unitOfWork, IContestClient contestClient) : IRegistrationService
{
    public async Task RegisterUserAsync(int contestStageId, string yandexIdLogin)
    {
        if (await unitOfWork.ContestStageRepository.GetAsync(contestStageId) is not { } contestStage)
            throw new ContestStageDoesNotExistException(contestStageId);

        if (contestStage.YandexContestId is not { } yandexContestId)
            throw new YandexContestIdNotSetException(contestStageId);

        if (await unitOfWork.ContestUserRepository.IsExistsAsync(contestStageId, yandexIdLogin))
            throw new UserIsAlreadyRegisteredException(contestStageId, yandexIdLogin);

        try
        {
            var contestUserId = await contestClient.RegisterParticipantByLoginAsync(yandexContestId, yandexIdLogin);
            unitOfWork.ContestUserRepository.Add(new(contestStage.Id, yandexIdLogin, contestUserId));
            await unitOfWork.SaveChangesAsync();
        }
        catch (InvalidUserException e)
        {
            throw new InvalidYandexUserException(e);
        }
    }

    public async Task UnregisterUserAsync(int contestStageId, string yandexIdLogin)
    {
        if (await unitOfWork.ContestStageRepository.GetAsync(contestStageId) is not { } contestStage)
            throw new ContestStageDoesNotExistException(contestStageId);

        if (contestStage.YandexContestId is not { } yandexContestId)
            throw new YandexContestIdNotSetException(contestStageId);


        var contestUser = await unitOfWork.ContestUserRepository.GetAsync(contestStageId, yandexIdLogin)
                       ?? throw new UserIsNotRegisteredException(contestStageId, yandexIdLogin);

        await contestClient.UnregisterParticipantAsync(yandexContestId, contestUser.ContestUserId);
        
        await unitOfWork.ContestUserRepository.DeleteAsync(new(contestStageId, yandexIdLogin));
    }
}
