namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Exceptions;

public class InvalidYandexUserException(Exception innerException) : Exception(innerException.Message, innerException);
