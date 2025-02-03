using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;
using CompilerLimit = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.CompilerLimit;
using ContestDescription = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestDescription;
using ContestProblem = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestProblem;
using ContestStandings = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestStandings;
using ContestStandingsTitle = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestStandingsTitle;
using ContestStatistics = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestStatistics;
using ContestType = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestType;
using ParticipantInfo = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ParticipantInfo;
using ParticipantStatus = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ParticipantStatus;
using ParticipationState = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ParticipationState;
using ProblemResult = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ProblemResult;
using Statement = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.Statement;
using SubmitInfo = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.SubmitInfo;
using UpsolvingAllowance = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.UpsolvingAllowance;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class ContestDataServiceImpl(IContestClient contestClient, IParticipantService participantService) : ContestDataService.ContestDataServiceBase
{
    public override Task<GetContestUrlResponse> GetContestUrl(GetContestUrlRequest request, ServerCallContext context) =>
        Task.FromResult<GetContestUrlResponse>(new()
        {
            ContestUrl = $"https://contest.yandex.ru/contest/{request.ContestId}/enter/"
        });

    public override async Task<GetContestResponse> GetContest(GetContestRequest request, ServerCallContext context)
    {
        var contestDescription = await contestClient.GetContestDescriptionAsync(request.ContestId);

        return new()
        {
            Result = contestDescription.MapContestDescription()
        };
    }

    public override async Task<GetProblemsResponse> GetProblems(GetProblemsRequest request, ServerCallContext context)
    {
        var contestProblems = await contestClient.GetContestProblemsAsync(request.ContestId);

        return new()
        {
            Problems = { contestProblems.Problems.Select(problem => problem.MapContestProblem()) }
        };
    }

    public override async Task<GetStandingsResponse> GetStandings(GetStandingsRequest request, ServerCallContext context)
    {
        var contestStandings = await contestClient.GetContestStandingsAsync(request.ContestId,
                                                                            forJudge: true,
                                                                            page: request.PageIndex,
                                                                            pageSize: request.PageSize,
                                                                            participantSearch: request.ParticipantSearch);
        return new()
        {
            Result = contestStandings.MapContestStandings()
        };
    }

    public override async Task<ParticipantStatusResponse> GetParticipantStatus(ParticipantStatusRequest request, ServerCallContext context)
    {
        var contestUserId = await GetContestParticipantAsync(request.ContestId, request.ParticipantLogin);

        var participantStatus = await contestClient.GetParticipantStatusAsync(request.ContestId, contestUserId);

        return new()
        {
            Result = participantStatus.MapParticipationStatus()
        };
    }

    private async Task<long> GetContestParticipantAsync(long contestId, string participantLogin) =>
        await participantService.GetContestUserIdAsync(contestId, participantLogin)
     ?? throw new RpcException(new(StatusCode.NotFound, "Contest participant not found"));
}

file static class MappingExtensions
{
    public static ContestProblem MapContestProblem(this YandexClient.Models.ContestProblem contestProblem) =>
        new()
        {
            Alias = contestProblem.Alias,
            Compilers = { contestProblem.Compilers },
            Id = contestProblem.Id,
            Limits = { contestProblem.Limits.Select(limit => limit.MapCompilerLimit()) },
            Name = contestProblem.Name,
            ProblemType = contestProblem.ProblemType,
            Statements = { contestProblem.Statements.Select(statement => statement.MapStatement()) },
            TestCount = contestProblem.TestCount
        };

    private static CompilerLimit MapCompilerLimit(this YandexClient.Models.CompilerLimit compilerLimit) =>
        new()
        {
            CompilerName = compilerLimit.CompilerName,
            IdlenessLimit = compilerLimit.IdlenessLimit,
            MemoryLimit = compilerLimit.MemoryLimit,
            OutputLimit = compilerLimit.OutputLimit,
            TimeLimit = compilerLimit.TimeLimit
        };

    private static Statement MapStatement(this YandexClient.Models.Statement statement) =>
        new()
        {
            Locale = statement.Locale,
            Path = statement.Path,
            Type = statement.Type switch
            {
                "TEX"      => StatementType.Tex,
                "PDF"      => StatementType.Pdf,
                "BINARY"   => StatementType.Binary,
                "OLYMP"    => StatementType.Olymp,
                "MARKDOWN" => StatementType.Markdown,
                _          => throw new ArgumentOutOfRangeException(nameof(statement), statement.Type, "Invalid statement type")
            }
        };

    public static ContestStandings MapContestStandings(this YandexClient.Models.ContestStandings contestStandings) =>
        new()
        {
            Rows = { contestStandings.Rows.Select(row => row.MapContestStandingRow()) },
            Statistics = contestStandings.Statistics.MapContestStatistics(),
            Titles = { contestStandings.Titles.Select(title => title.MapContestStandingsTitle()) }
        };

    private static ContestStandingRow MapContestStandingRow(this ContestStandingsRow contestStandingsRow) =>
        new()
        {
            ParticipantInfo = contestStandingsRow.ParticipantInfo.MapParticipantInfo(),
            PlaceFrom = { contestStandingsRow.PlaceFrom },
            PlaceTo = { contestStandingsRow.PlaceTo },
            ProblemResults = { contestStandingsRow.ProblemResults.Select(result => result.MapProblemResult()) },
            Score = double.TryParse(contestStandingsRow.Score, CultureInfo.GetCultureInfo("ru-RU"), out var score)
                        ? score
                        : null
        };

    private static ParticipantInfo MapParticipantInfo(this YandexClient.Models.ParticipantInfo participantInfo) =>
        new()
        {
            Id = participantInfo.Id,
            Login = participantInfo.Login,
            Name = participantInfo.Name,
            StartTime = participantInfo.StartTime,
            Uid = participantInfo.Uid
        };

    private static ProblemResult MapProblemResult(this YandexClient.Models.ProblemResult problemResult) =>
        new()
        {
            Score = double.TryParse(problemResult.Score, CultureInfo.GetCultureInfo("ru-RU"), out var score)
                        ? score
                        : null,
            Status = problemResult.Status switch
            {
                "ACCEPTED"      => SubmissionStatus.Accepted,
                "NOT_ACCEPTED"  => SubmissionStatus.NotAccepted,
                "NOT_SUBMITTED" => SubmissionStatus.NotSubmitted,
                _               => SubmissionStatus.Other
            },
            SubmissionCount = int.Parse(problemResult.SubmissionCount),
            SubmitDelay = TimeSpan.FromSeconds(problemResult.SubmitDelay).ToDuration()
        };

    private static ContestStatistics MapContestStatistics(this YandexClient.Models.ContestStatistics contestStatistics) =>
        new()
        {
            LastSubmit = contestStatistics.LastSubmit.MapSubmitInfo(),
            LastSuccess = contestStatistics.LastSuccess.MapSubmitInfo()
        };

    private static SubmitInfo MapSubmitInfo(this YandexClient.Models.SubmitInfo submitInfo) =>
        new()
        {
            ParticipantId = submitInfo.ParticipantId,
            ParticipantName = submitInfo.ParticipantName,
            ProblemTitle = submitInfo.ProblemTitle,
            SubmitTime = TimeSpan.FromMilliseconds(submitInfo.SubmitTime).ToDuration()
        };

    private static ContestStandingsTitle MapContestStandingsTitle(this YandexClient.Models.ContestStandingsTitle contestStandingsTitle) =>
        new()
        {
            Name = contestStandingsTitle.Name,
            Title = contestStandingsTitle.Title
        };

    public static ParticipantStatus MapParticipationStatus(this YandexClient.Models.ParticipantStatus participantStatus) =>
        new()
        {
            Name = participantStatus.Name,
            StartTime = participantStatus.StartTime?.ToTimestamp(),
            FinishTime = participantStatus.FinishTime?.ToTimestamp(),
            LeftTimeMilliseconds = participantStatus.LeftTimeMilliseconds,
            State = participantStatus.State.MapParticipationState()
        };

    private static ParticipationState MapParticipationState(this YandexClient.Models.ParticipationState participationState) =>
        participationState switch
        {
            YandexClient.Models.ParticipationState.InProgress => ParticipationState.InProgress,
            YandexClient.Models.ParticipationState.Finished   => ParticipationState.Finished,
            _                                                 => ParticipationState.NotStarted
        };

    public static ContestDescription MapContestDescription(
        this YandexClient.Models.ContestDescription contestDescription) =>
        new()
        {
            Name = contestDescription.Name,
            StartTime = contestDescription.StartTime.ToTimestamp(),
            Duration = TimeSpan.FromSeconds(contestDescription.DurationSeconds).ToDuration(),
            FreezeTime = contestDescription.FreezeTimeSeconds is { } freezeTimeSeconds
                             ? TimeSpan.FromSeconds(freezeTimeSeconds)
                                       .ToDuration()
                             : null,
            Type = contestDescription.Type.MapContestType(),
            UpsolvingAllowance = contestDescription.UpsolvingAllowance.MapUpsolvingAllowance()
        };

    private static ContestType MapContestType(this YandexClient.Models.ContestType contestType) =>
        contestType switch
        {
            YandexClient.Models.ContestType.Usual   => ContestType.Usual,
            YandexClient.Models.ContestType.Virtual => ContestType.Virtual,
            _                                       => throw new ArgumentOutOfRangeException(nameof(contestType), contestType, "Invalid contest type")
        };

    private static UpsolvingAllowance MapUpsolvingAllowance(this YandexClient.Models.UpsolvingAllowance upsolvingAllowance) =>
        upsolvingAllowance switch
        {
            YandexClient.Models.UpsolvingAllowance.Prohibited                    => UpsolvingAllowance.Prohibited,
            YandexClient.Models.UpsolvingAllowance.AllowedAfterParticipationEnds => UpsolvingAllowance.AllowedAfterParticipationEnds,
            YandexClient.Models.UpsolvingAllowance.AllowedAfterContestEnds       => UpsolvingAllowance.AllowedAfterContestEnds,
            _                                                                    => throw new ArgumentOutOfRangeException(nameof(upsolvingAllowance), upsolvingAllowance, "Invalid upsolving allowance type")
        };
}
