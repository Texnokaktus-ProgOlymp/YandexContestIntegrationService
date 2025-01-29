using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;

public class UserIsAlreadyRegisteredException(int contestStageId, string yandexIdLogin)
    : RpcApplicationException(ErrorType.UserIsAlreadyRegistered,
                              $"User {yandexIdLogin} is already registered for Contest Stage {contestStageId}");
