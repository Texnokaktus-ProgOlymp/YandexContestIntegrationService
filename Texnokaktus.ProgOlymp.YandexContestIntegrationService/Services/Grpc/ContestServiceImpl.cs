using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class ContestServiceImpl(IContestStageService contestStageService) : ContestService.ContestServiceBase
{
    public override async Task<GetAllContestsResponse> GetAllContests(Empty request, ServerCallContext context)
    {
        var contestStages = await contestStageService.GetContestStagesAsync();

        return new()
        {
            Result =
            {
                contestStages.Select(stage => new Contest
                {
                    Id = stage.Id,
                    YandexContestId = stage.YandexContestId
                })
            }
        };
    }

    public override async Task<Empty> AddContest(AddContestRequest request, ServerCallContext context)
    {
        await contestStageService.AddContestStageAsync(new(request.ContestId, request.YandexContestId));

        return new();
    }
}
