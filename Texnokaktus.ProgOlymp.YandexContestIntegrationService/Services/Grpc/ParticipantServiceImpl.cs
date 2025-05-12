using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using YandexContestClient.Client;
using YandexContestClient.Client.Models;
using BriefRunReport = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.BriefRunReport;
using ParticipantStats = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ParticipantStats;
using ParticipantStatus = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ParticipantStatus;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class ParticipantServiceImpl(ContestClient contestClient, IParticipantService participantService) : ParticipantService.ParticipantServiceBase
{
    public override async Task<ParticipantStatusResponse> GetParticipantStatus(ParticipantStatusRequest request, ServerCallContext context)
    {
        var contestUserId = await GetContestParticipantAsync(request.ContestId, request.ParticipantLogin);

        var participantStatus = await contestClient.Contests[request.ContestId].Participants[contestUserId].GetAsync();

        return new()
        {
            Result = participantStatus?.MapParticipationStatus()
        };
    }

    public override async Task<ParticipantStatsResponse> GetParticipantStats(ParticipantStatsRequest request, ServerCallContext context)
    {
        var contestUserId = await GetContestParticipantAsync(request.ContestId, request.ParticipantLogin);

        var stats = await contestClient.Contests[request.ContestId].Participants[contestUserId].Stats.GetAsync();

        return new()
        {
            Result = stats?.MapParticipantStats()
        };
    }

    private async Task<long> GetContestParticipantAsync(long contestId, string participantLogin) =>
        await participantService.GetContestUserIdAsync(contestId, participantLogin)
     ?? throw new RpcException(new(StatusCode.NotFound, "Contest participant not found"));
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
            LeftTimeMilliseconds = participantStatus.ParticipantLeftTimeMillis ?? 0,
            State = participantStatus.ContestState.MapParticipationState()
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
            _                                          => throw new ArgumentOutOfRangeException(nameof(participationState), participationState, null)
        };
}
