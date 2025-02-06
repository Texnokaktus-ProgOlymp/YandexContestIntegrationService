namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IRegistrationService
{
    Task RegisterUserAsync(long contestStageId, string yandexIdLogin, string? participantDisplayName);
    Task UnregisterUserAsync(long contestStageId, string yandexIdLogin);
}
