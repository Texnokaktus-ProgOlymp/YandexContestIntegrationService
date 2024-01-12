using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

public interface IApplicationStateService
{
    Task<IEnumerable<ApplicationAction>> GetAvailableActionsAsync(int applicationId);
    Task InvokeApplicationAction(int applicationId, ApplicationAction action);
}
