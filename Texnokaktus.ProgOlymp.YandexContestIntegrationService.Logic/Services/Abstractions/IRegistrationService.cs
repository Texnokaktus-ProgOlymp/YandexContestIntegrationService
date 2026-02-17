namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IRegistrationService
{
    Task<long> RegisterUserAsync(long contestStageId, string yandexIdLogin, string? participantDisplayName, int participantId);
    Task UnregisterUserAsync(long contestStageId, int participantId);
}
