namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;

public class InvalidUserException : Exception
{
    public InvalidUserException(string? message) : base(message)
    {
    }

    public InvalidUserException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
