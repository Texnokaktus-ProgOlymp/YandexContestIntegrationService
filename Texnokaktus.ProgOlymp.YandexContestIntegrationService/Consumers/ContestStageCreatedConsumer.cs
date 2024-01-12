using MassTransit;
using Texnokaktus.ProgOlymp.Common.Contracts.Messages;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Consumers;

public class ContestStageCreatedConsumer(IContestStageService contestStageService) : IConsumer<ContestStageCreated>
{
    public async Task Consume(ConsumeContext<ContestStageCreated> context) =>
        await contestStageService.AddContestStageAsync(context.Message.ContestStageId);
}
