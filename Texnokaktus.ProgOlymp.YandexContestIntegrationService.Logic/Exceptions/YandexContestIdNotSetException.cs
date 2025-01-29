using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;

public class YandexContestIdNotSetException(int contestStageId)
    : RpcApplicationException(ErrorType.YandexContestIdNotSet,
                              $"Yandex Contest ID is not set for Contest Stage {contestStageId}");
