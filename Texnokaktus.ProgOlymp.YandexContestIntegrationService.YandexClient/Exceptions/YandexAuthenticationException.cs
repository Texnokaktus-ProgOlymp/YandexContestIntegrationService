namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;

public class YandexAuthenticationException : Exception
{
    public YandexAuthenticationException(string? message) : base(message)
    {
    }

    public YandexAuthenticationException(string? message, Exception innerException) : base(message, innerException)
    {
    }
}
