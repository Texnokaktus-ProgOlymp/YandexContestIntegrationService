using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;

public class InvalidYandexUserException(Exception innerException)
    : RpcApplicationException(ErrorType.InvalidUser, innerException.Message, innerException);
