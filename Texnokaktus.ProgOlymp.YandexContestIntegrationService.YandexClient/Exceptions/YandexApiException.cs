namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;

public class YandexApiException : Exception
{
    public YandexApiException(string? message) : base(message)
    {
    }

    public YandexApiException(string? message, Exception innerException) : base(message, innerException)
    {
    }
}
