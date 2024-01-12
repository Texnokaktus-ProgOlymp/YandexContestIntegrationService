using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Mapping;

internal static class MappingExtensions
{
    public static ContestStageApplication MapContestStageApplication(this DataAccess.Entities.ContestStageApplication application) =>
        new()
        {
            Id = application.Id,
            TransactionId = application.TransactionId,
            YandexIdLogin = application.YandexIdLogin,
            ContestStageId = application.ContestStageId,
            YandexContestId = application.ContestStage.YandexContestId,
            State = MapApplicationState(application.State),
            CreatedUtc = application.CreatedUtc
        };

    public static ApplicationState MapApplicationState(this DataAccess.Entities.ApplicationState applicationState) =>
        applicationState switch
        {
            DataAccess.Entities.ApplicationState.Pending => ApplicationState.Pending,
            DataAccess.Entities.ApplicationState.Finished => ApplicationState.Finished,
            DataAccess.Entities.ApplicationState.Failed => ApplicationState.Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(applicationState), applicationState, null)
        };

    public static DataAccess.Entities.ApplicationState MapApplicationState(this ApplicationState applicationState) =>
        applicationState switch
        {
            ApplicationState.Pending    => DataAccess.Entities.ApplicationState.Pending,
            ApplicationState.Finished   => DataAccess.Entities.ApplicationState.Finished,
            ApplicationState.Failed     => DataAccess.Entities.ApplicationState.Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(applicationState), applicationState, null)
        };
}
