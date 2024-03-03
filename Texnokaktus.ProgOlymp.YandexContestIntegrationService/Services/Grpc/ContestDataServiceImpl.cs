using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class ContestDataServiceImpl(IContestClient contestClient) : ContestDataService.ContestDataServiceBase
{
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
                                                                            pageSize: request.PageSize);
        return new()
        {
            Result = contestStandings.MapContestStandings()
        };
    }
}

file static class MappingExtensions
{
    public static ContestProblem MapContestProblem(this YandexClient.Models.ContestProblem contestProblem) =>
        new()
        {
            Alias = contestProblem.Alias,
            Compilers = { contestProblem.Compilers },
            Id = contestProblem.Id,
            Limits = { contestProblem.Limits.Select(limit => limit.MapCompilerLimit())  },
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
                _          => throw new ArgumentOutOfRangeException(nameof(YandexClient.Models.Statement.Type),
                                                                    statement.Type,
                                                                    "Invalid statement type")
            }
        };

    public static ContestStandings MapContestStandings(this YandexClient.Models.ContestStandings contestStandings) =>
        new()
        {
            Rows = { contestStandings.Rows.Select(row => row.MapContestStandingRow()) },
            Statistics = contestStandings.Statistics.MapContestStatistics(),
            Titles = { contestStandings.Titles.Select(title => title.MapContestStandingsTitle()) }
        };

    private static ContestStandingRow MapContestStandingRow(this YandexClient.Models.ContestStandingsRow contestStandingsRow) =>
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
}
