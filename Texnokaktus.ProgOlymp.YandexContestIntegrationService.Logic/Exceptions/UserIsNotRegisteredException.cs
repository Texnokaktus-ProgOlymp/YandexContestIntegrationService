using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;

public class UserIsNotRegisteredException(int contestStageId, string yandexIdLogin)
    : RpcApplicationException(ErrorType.UserIsNotRegistered,
                              $"User {yandexIdLogin} is not registered for Contest Stage {contestStageId}");
