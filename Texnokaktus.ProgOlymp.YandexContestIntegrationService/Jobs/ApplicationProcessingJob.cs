using Quartz;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Exceptions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Jobs;

public class ApplicationProcessingJob(ILogger<ApplicationProcessingJob> logger,
                                      IApplicationService applicationService,
                                      IApplicationStateService applicationStateService,
                                      IContestClient contestClient) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        foreach (var application in await applicationService.GetPendingApplicationsAsync())
        {
            if (application.YandexContestId is null)
            {
                logger.LogWarning("Cannot process the application because yhe Yandex Contest ID for the contest stage {ContestStageId} is not set", application.ContestStageId);
                continue;
            }

            var success = true;
            try
            {
                await contestClient.RegisterParticipantByLogin(application.YandexContestId.Value,
                                                               application.YandexIdLogin);
            }
            catch (InvalidUserException e)
            {
                logger.LogError(e, "Invalid user login");
                await applicationStateService.InvokeApplicationAction(application.Id, ApplicationAction.Fail);
                success = false;
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred when processing the application");
                await applicationStateService.InvokeApplicationAction(application.Id, ApplicationAction.Fail);
                success = false;
            }

            if (success)
                await applicationStateService.InvokeApplicationAction(application.Id, ApplicationAction.Finish);
        }
    }
}
