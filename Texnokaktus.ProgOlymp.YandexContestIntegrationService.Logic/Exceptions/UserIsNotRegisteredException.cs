namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;

public class UserIsNotRegisteredException(long contestStageId, string yandexIdLogin)
    : Exception($"User {yandexIdLogin} is not registered for Contest Stage {contestStageId}");
