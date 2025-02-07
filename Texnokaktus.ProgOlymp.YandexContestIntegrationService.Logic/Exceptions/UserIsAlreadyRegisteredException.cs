namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;

public class UserIsAlreadyRegisteredException(long contestStageId, string yandexIdLogin)
    : Exception($"User {yandexIdLogin} is already registered for Contest Stage {contestStageId}");
