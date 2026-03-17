using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Exceptions;
using YandexContestClient.Client;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class RegistrationServiceImpl(ContestClient contestClient, ILogger<RegistrationServiceImpl> logger) : RegistrationService.RegistrationServiceBase
{
    public override async Task<RegisterParticipantResponse> RegisterParticipant(RegisterParticipantRequest request, ServerCallContext context)
    {
        var contestUserId = await contestClient.Contests[request.ContestStageId]
                                               .Participants
                                               .PostAsync(configuration => configuration.QueryParameters.Login = request.YandexIdLogin)
                         ?? throw new YandexApiException("Unable to get Paricipant Id");

        if (request.DisplayName is not null)
            await SetParticipantDisplayNameAsync(request.ContestStageId, contestUserId, request.DisplayName);
        
        return new()
        {
            ContestParticipantId = contestUserId
        };
    }

    public override async Task<Empty> UnregisterParticipant(UnregisterParticipantRequest request, ServerCallContext context)
    {
        await contestClient.Contests[request.ContestStageId]
                           .Participants[request.ContestParticipantId]
                           .DeleteAsync();
        return new();
    }

    private async Task SetParticipantDisplayNameAsync(long yandexContestId, long yandexParticipantId, string displayName)
    {
        try
        {
            await contestClient.Contests[yandexContestId]
                               .Participants[yandexParticipantId]
                               .PatchAsync(new()
                                {
                                    DisplayedName = displayName
                                });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to update participant's display name");
        }
    }
}
