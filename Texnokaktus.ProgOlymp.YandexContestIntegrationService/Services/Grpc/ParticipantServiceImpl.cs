using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Common;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Extensions;
using YandexContestClient.Client;
using YandexContestClient.Client.Models;
using BriefRunReport = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.BriefRunReport;
using ParticipantStats = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ParticipantStats;
using ParticipantStatus = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ParticipantStatus;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class ParticipantServiceImpl(ContestClient contestClient) : ParticipantService.ParticipantServiceBase
{
    public override async Task<ContestParticipationResponse> GetContestOwnerParticipation(ContestParticipationRequest request, ServerCallContext context)
    {
        var participantStatus = await contestClient.Contests[request.ContestId].Participation.GetAsync(cancellationToken: context.CancellationToken);

        return new()
        {
            Result = participantStatus?.MapParticipationStatus()
        };
    }

    public override async Task<ContestParticipantsResponse> GetContestParticipants(ContestParticipantsRequest request, ServerCallContext context)
    {
        var participantInfos = await contestClient.Contests[request.ContestId].Participants.GetAsync(cancellationToken: context.CancellationToken);

        return new()
        {
            Result =
            {
                participantInfos?.Select(info => info.MapParticipantInfo()) ?? []
            }
        };
    }

    public override async Task<ParticipantStatusResponse> GetParticipantStatus(ParticipantStatusRequest request, ServerCallContext context)
    {
        var participantStatus = await contestClient.Contests[request.ContestId].Participants[request.ContestParticipantId].GetAsync(cancellationToken: context.CancellationToken);

        return new()
        {
            Result = participantStatus?.MapParticipationStatus()
        };
    }

    public override async Task<ParticipantStatsResponse> GetParticipantStats(ParticipantStatsRequest request, ServerCallContext context)
    {
        var stats = await contestClient.Contests[request.ContestId].Participants[request.ContestParticipantId].Stats.GetAsync(cancellationToken: context.CancellationToken);

        return new()
        {
            Result = stats?.MapParticipantStats()
        };
    }
}

file static class MappingExtensions
{
    public static ParticipantStatus MapParticipationStatus(this YandexContestClient.Client.Models.ParticipantStatus participantStatus) =>
        new()
        {
            Name = participantStatus.ParticipantName,
            StartTime = DateTimeOffset.TryParse(participantStatus.ParticipantStartTime, out var startTime)
                            ? startTime.ToTimestamp()
                            : null,
            FinishTime = DateTimeOffset.TryParse(participantStatus.ParticipantFinishTime, out var finishTime)
                             ? finishTime.ToTimestamp()
                             : null,
            LeftTime = TimeSpan.FromMilliseconds(participantStatus.ParticipantLeftTimeMillis ?? 0).ToDuration(),
            State = participantStatus.ContestState.MapParticipationState(),
            ContestStartTime = DateTimeOffset.TryParse(participantStatus.ContestStartTime, out var contestStartTime)
                            ? contestStartTime.ToTimestamp()
                            : null,
            ContestFinishTime = DateTimeOffset.TryParse(participantStatus.ContestFinishTime, out var contestFinishTime)
                             ? contestFinishTime.ToTimestamp()
                             : null
        };

    public static ParticipantStats MapParticipantStats(this YandexContestClient.Client.Models.ParticipantStats stats) =>
        new()
        {
            StartedAt = DateTimeOffset.TryParse(stats.StartedAt, out var startedAt) ? startedAt.ToTimestamp() : null,
            FirstSubmissionTime = DateTimeOffset.TryParse(stats.FirstSubmissionTime, out var firstSubmissionTime)
                                      ? firstSubmissionTime.ToTimestamp()
                                      : null,
            Runs = { stats.Runs?.Select(run => run.MapBriefRunReport()) }
        };

    private static BriefRunReport MapBriefRunReport(this YandexContestClient.Client.Models.BriefRunReport run) =>
        new()
        {
            RunId = run.RunId ?? 0L,
            ProblemId = run.ProblemId,
            ProblemAlias = run.ProblemAlias,
            Compiler = run.Compiler,
            SubmissionTime = DateTimeOffset.TryParse(run.SubmissionTime, out var x) ? x.ToTimestamp() : null,
            TimeFromStart = TimeSpan.FromMilliseconds(run.TimeFromStart ?? 0L).ToDuration(),
            Verdict = run.Verdict,
            TestNumber = run.TestNumber ?? 0,
            MaxTimeUsage = run.MaxTimeUsage ?? 0L,
            MaxMemoryUsage = run.MaxMemoryUsage ?? 0L,
            Score = run.Score
        };

    private static ParticipationState MapParticipationState(this ParticipantStatus_contestState? participationState) =>
        participationState switch
        {
            ParticipantStatus_contestState.IN_PROGRESS => ParticipationState.InProgress,
            ParticipantStatus_contestState.FINISHED    => ParticipationState.Finished,
            ParticipantStatus_contestState.NOT_STARTED => ParticipationState.NotStarted,
            _                                          => throw new ArgumentOutOfRangeException(nameof(participationState), participationState, $"Unable to map {nameof(ParticipationState)}")
        };
}
