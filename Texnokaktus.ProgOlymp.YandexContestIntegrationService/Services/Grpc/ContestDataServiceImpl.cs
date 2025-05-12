using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using YandexContestClient.Client;
using YandexContestClient.Client.Models;
using CompilerLimit = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.CompilerLimit;
using ContestDescription = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestDescription;
using ContestProblem = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestProblem;
using ContestStandings = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestStandings;
using ContestStandingsTitle = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestStandingsTitle;
using ContestStatistics = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestStatistics;
using ContestType = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ContestType;
using ParticipantInfo = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ParticipantInfo;
using ProblemResult = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.ProblemResult;
using Statement = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.Statement;
using SubmitInfo = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.SubmitInfo;
using UpsolvingAllowance = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.UpsolvingAllowance;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class ContestDataServiceImpl(ContestClient contestClient) : ContestDataService.ContestDataServiceBase
{
    public override Task<GetContestUrlResponse> GetContestUrl(GetContestUrlRequest request, ServerCallContext context) =>
        Task.FromResult<GetContestUrlResponse>(new()
        {
            ContestUrl = $"https://contest.yandex.ru/contest/{request.ContestId}/enter/"
        });

    public override async Task<GetContestResponse> GetContest(GetContestRequest request, ServerCallContext context)
    {
        var contestDescription = await contestClient.Contests[request.ContestId].GetAsync();

        return new()
        {
            Result = contestDescription?.MapContestDescription()
        };
    }

    public override async Task<GetProblemsResponse> GetProblems(GetProblemsRequest request, ServerCallContext context)
    {
        var contestProblems = await contestClient.Contests[request.ContestId].Problems.GetAsync();

        return new()
        {
            Problems = { contestProblems?.Problems?.Select(problem => problem.MapContestProblem()) }
        };
    }

    public override async Task<GetStandingsResponse> GetStandings(GetStandingsRequest request, ServerCallContext context)
    {
        var contestStandings = await contestClient.Contests[request.ContestId]
                                                  .Standings
                                                  .GetAsync(configuration =>
                                                   {
                                                       configuration.QueryParameters.ForJudge = true;
                                                       configuration.QueryParameters.Page = request.PageIndex;
                                                       configuration.QueryParameters.PageSize = request.PageSize;
                                                       configuration.QueryParameters.ParticipantSearch = request.ParticipantSearch;
                                                   });

        return new()
        {
            Result = contestStandings?.MapContestStandings()
        };
    }
}

file static class MappingExtensions
{
    public static ContestProblem MapContestProblem(this YandexContestClient.Client.Models.ContestProblem contestProblem) =>
        new()
        {
            Alias = contestProblem.Alias,
            Compilers = { contestProblem.Compilers },
            Id = contestProblem.Id,
            Limits = { contestProblem.Limits?.Select(limit => limit.MapCompilerLimit()) },
            Name = contestProblem.Name,
            ProblemType = contestProblem.ProblemType,
            Statements = { contestProblem.Statements?.Select(statement => statement.MapStatement()) },
            TestCount = contestProblem.TestCount ?? 0
        };

    private static CompilerLimit MapCompilerLimit(this YandexContestClient.Client.Models.CompilerLimit compilerLimit) =>
        new()
        {
            CompilerName = compilerLimit.CompilerName,
            IdlenessLimit = compilerLimit.IdlenessLimit ?? 0L,
            MemoryLimit = compilerLimit.MemoryLimit ?? 0L,
            OutputLimit = compilerLimit.OutputLimit ?? 0L,
            TimeLimit = compilerLimit.TimeLimit ?? 0L
        };

    private static Statement MapStatement(this YandexContestClient.Client.Models.Statement statement) =>
        new()
        {
            Locale = statement.Locale,
            Path = statement.Path,
            Type = statement.Type.MapStatementType()
        };

    private static StatementType MapStatementType(this Statement_type? statementType) =>
        statementType switch
        {
            Statement_type.TEX      => StatementType.Tex,
            Statement_type.PDF      => StatementType.Pdf,
            Statement_type.BINARY   => StatementType.Binary,
            Statement_type.MARKDOWN => StatementType.Markdown,
            null                    => throw new ArgumentNullException(nameof(statementType)),
            _                       => throw new ArgumentOutOfRangeException(nameof(statementType), statementType, null)
        };

    public static ContestStandings MapContestStandings(this YandexContestClient.Client.Models.ContestStandings contestStandings) =>
        new()
        {
            Rows = { contestStandings.Rows?.Select(row => row.MapContestStandingRow()) },
            Statistics = contestStandings.Statistics?.MapContestStatistics(),
            Titles = { contestStandings.Titles?.Select(title => title.MapContestStandingsTitle()) }
        };

    private static ContestStandingRow MapContestStandingRow(this ContestStandingsRow contestStandingsRow) =>
        new()
        {
            ParticipantInfo = contestStandingsRow.ParticipantInfo?.MapParticipantInfo(),
            PlaceFrom = { contestStandingsRow.PlaceFrom?.Select(i => i!.Value) },
            PlaceTo = { contestStandingsRow.PlaceTo?.Select(i => i!.Value) },
            ProblemResults = { contestStandingsRow.ProblemResults?.Select(result => result.MapProblemResult()) },
            Score = double.TryParse(contestStandingsRow.Score, CultureInfo.GetCultureInfo("ru-RU"), out var score)
                        ? score
                        : null
        };

    private static ParticipantInfo MapParticipantInfo(this YandexContestClient.Client.Models.ParticipantInfo participantInfo) =>
        new()
        {
            Id = participantInfo.Id ?? 0L,
            Login = participantInfo.Login,
            Name = participantInfo.Name,
            StartTime = participantInfo.StartTime,
            Uid = participantInfo.Uid
        };

    private static ProblemResult MapProblemResult(this YandexContestClient.Client.Models.ProblemResult problemResult) =>
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
            SubmissionCount = int.TryParse(problemResult.SubmissionCount, out var result) ? result : 0,
            SubmitDelay = TimeSpan.FromSeconds(problemResult.SubmitDelay ?? 0L).ToDuration()
        };

    private static ContestStatistics MapContestStatistics(this YandexContestClient.Client.Models.ContestStatistics contestStatistics) =>
        new()
        {
            LastSubmit = contestStatistics.LastSubmit?.MapSubmitInfo(),
            LastSuccess = contestStatistics.LastSuccess?.MapSubmitInfo()
        };

    private static SubmitInfo MapSubmitInfo(this YandexContestClient.Client.Models.SubmitInfo submitInfo) =>
        new()
        {
            ParticipantId = submitInfo.ParticipantId ?? 0L,
            ParticipantName = submitInfo.ParticipantName,
            ProblemTitle = submitInfo.ProblemTitle,
            SubmitTime = TimeSpan.FromMilliseconds(submitInfo.SubmitTime ?? 0L).ToDuration()
        };

    private static ContestStandingsTitle MapContestStandingsTitle(this YandexContestClient.Client.Models.ContestStandingsTitle contestStandingsTitle) =>
        new()
        {
            Name = contestStandingsTitle.Name,
            Title = contestStandingsTitle.Title
        };

    public static ContestDescription MapContestDescription(this YandexContestClient.Client.Models.ContestDescription contestDescription) =>
        new()
        {
            Name = contestDescription.Name,
            StartTime = DateTime.TryParse(contestDescription.StartTime, out var dateTime)
                            ? dateTime.ToTimestamp()
                            : null,
            Duration = contestDescription.Duration is { } duration ? TimeSpan.FromSeconds(duration).ToDuration() : null,
            FreezeTime = contestDescription.FreezeTime is { } freezeTimeSeconds
                             ? TimeSpan.FromSeconds(freezeTimeSeconds).ToDuration()
                             : null,
            Type = contestDescription.Type.MapContestType(),
            UpsolvingAllowance = contestDescription.UpsolvingAllowance.MapUpsolvingAllowance()
        };

    private static ContestType MapContestType(this ContestDescription_type? contestType) =>
        contestType switch
        {
            ContestDescription_type.USUAL   => ContestType.Usual,
            ContestDescription_type.VIRTUAL => ContestType.Virtual,
            null                            => throw new ArgumentNullException(nameof(contestType)),
            _                               => throw new ArgumentOutOfRangeException(nameof(contestType), contestType, "Invalid contest type")
        };

    private static UpsolvingAllowance MapUpsolvingAllowance(this ContestDescription_upsolvingAllowance? upsolvingAllowance) =>
        upsolvingAllowance switch
        {
            ContestDescription_upsolvingAllowance.PROHIBITED                       => UpsolvingAllowance.Prohibited,
            ContestDescription_upsolvingAllowance.ALLOWED_AFTER_PARTICIPATION_ENDS => UpsolvingAllowance.AllowedAfterParticipationEnds,
            ContestDescription_upsolvingAllowance.ALLOWED_AFTER_CONTEST_ENDS       => UpsolvingAllowance.AllowedAfterContestEnds,
            null                                                                   => throw new ArgumentNullException(nameof(upsolvingAllowance)),
            _                                                                      => throw new ArgumentOutOfRangeException(nameof(upsolvingAllowance), upsolvingAllowance, "Invalid upsolving allowance type")
        };
}
