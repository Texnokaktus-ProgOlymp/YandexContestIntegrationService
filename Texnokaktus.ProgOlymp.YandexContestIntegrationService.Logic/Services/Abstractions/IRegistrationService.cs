namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IRegistrationService
{
    Task RegisterUserAsync(int contestStageId, string yandexIdLogin);
    Task UnregisterUserAsync(int contestStageId, string yandexIdLogin);
}
