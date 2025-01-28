namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IRegistrationService
{
    Task<string> RegisterUserAsync(int contestStageId, string yandexIdLogin, string? participantDisplayName);
    Task UnregisterUserAsync(int contestStageId, string yandexIdLogin);
}
