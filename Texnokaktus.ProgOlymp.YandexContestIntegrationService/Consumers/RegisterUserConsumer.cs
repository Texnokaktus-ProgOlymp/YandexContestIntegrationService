using MassTransit;
using Texnokaktus.ProgOlymp.Common.Contracts.Messages.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Consumers;

public class RegisterUserConsumer(IApplicationService applicationService) : IConsumer<RegisterUser>
{
    public async Task Consume(ConsumeContext<RegisterUser> context) =>
        await applicationService.AddApplicationAsync(context.Message.TransactionId,
                                                     context.Message.ContestStageId,
                                                     context.Message.YandexIdLogin);
}
