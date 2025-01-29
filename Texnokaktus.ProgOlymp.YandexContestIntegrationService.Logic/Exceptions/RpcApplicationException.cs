using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;

public class RpcApplicationException : Exception
{
    public RpcApplicationException(ErrorType errorType, string? message) : base(message)
    {
        ErrorType = errorType;
    }

    public RpcApplicationException(ErrorType errorType,
                                   string? message,
                                   Exception? innerException) : base(message, innerException)
    {
        ErrorType = errorType;
    }

    public ErrorType ErrorType { get; set; }
}
