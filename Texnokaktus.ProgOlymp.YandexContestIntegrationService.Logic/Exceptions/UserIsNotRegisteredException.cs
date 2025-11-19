namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;

public class UserIsNotRegisteredException(long contestStageId, int participantId)
    : Exception($"User {participantId} is not registered for Contest Stage {contestStageId}");
