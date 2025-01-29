using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;

public class ContestStageDoesNotExistException(int contestStageId)
    : RpcApplicationException(ErrorType.ContestStageNotExists, $"Contest stage with Id {contestStageId} does not exist");
