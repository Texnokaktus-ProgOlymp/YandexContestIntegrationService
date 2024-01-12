using System.Collections.Frozen;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Mapping;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;

internal class ApplicationStateService(IUnitOfWork unitOfWork) : IApplicationStateService
{
    private static readonly FrozenDictionary<ApplicationState, (ApplicationAction action, ApplicationState newState)[]> Transitions = new[]
        {
            KeyValuePair.Create(ApplicationState.Pending,
                                new[]
                                {
                                    (ApplicationAction.Finish, ApplicationState.Finished),
                                    (ApplicationAction.Fail, ApplicationState.Failed)
                                }),
            KeyValuePair.Create(ApplicationState.Finished,
                                new[]
                                {
                                    (ApplicationAction.Redo, ApplicationState.Pending)
                                }),
            KeyValuePair.Create(ApplicationState.Failed,
                                new[]
                                {
                                    (ApplicationAction.Redo, ApplicationState.Pending)
                                })
        }.ToFrozenDictionary();
    
    public async Task<IEnumerable<ApplicationAction>> GetAvailableActionsAsync(int applicationId)
    {
        var applicationState = (await unitOfWork.ContestStageApplicationRepository.GetStateAsync(applicationId))?.MapApplicationState();
        if (!applicationState.HasValue) return Enumerable.Empty<ApplicationAction>();

        return Transitions.TryGetValue(applicationState.Value, out var transitions)
                   ? transitions.Select(t => t.action)
                   : Enumerable.Empty<ApplicationAction>();
    }

    public async Task InvokeApplicationAction(int applicationId, ApplicationAction action)
    {
        var applicationState = (await unitOfWork.ContestStageApplicationRepository.GetStateAsync(applicationId))?.MapApplicationState()
                            ?? throw new("Could not find the current state of the application");

        if (!Transitions.TryGetValue(applicationState, out var transitions))
            throw new NotSupportedException("The current application state is not supported");

        if (transitions.All(t => t.action != action))
            throw new NotSupportedException("The transition is not supported");
        
        var newState = transitions.Single(t => t.action == action).newState;

        await unitOfWork.ContestStageApplicationRepository.SetStateAsync(applicationId, newState.MapApplicationState());

        await unitOfWork.SaveChangesAsync();
    }
}
