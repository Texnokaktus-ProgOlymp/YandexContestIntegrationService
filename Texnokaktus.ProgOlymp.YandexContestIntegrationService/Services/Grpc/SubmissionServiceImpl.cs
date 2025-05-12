using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using YandexContestClient.Client;
using YandexContestClient.Client.Models;
using Submission = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.Submission;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class SubmissionServiceImpl(ContestClient client) : SubmissionService.SubmissionServiceBase
{
    public override async Task GetSubmissions(GetSubmissionsRequest request,
                                              IServerStreamWriter<Submission> responseStream,
                                              ServerCallContext context)
    {
        var currentPage = 0;
        var processed = 0;

        Submissions? submissions;

        do
        {
            submissions = await client.Contests[request.ContestStageId]
                                      .Submissions
                                      .GetAsync(configuration => configuration.QueryParameters.Page = ++currentPage,
                                                context.CancellationToken);

            foreach (var submission in submissions?.SubmissionsProp ?? [])
            {
                await responseStream.WriteAsync(submission.MapSubmission(), context.CancellationToken);
                processed++;
            }
        } while (processed < submissions?.Count);
    }
}

file static class MappingExtensions
{
    public static Submission MapSubmission(this YandexContestClient.Client.Models.Submission submission) =>
        new()
        {
            Id = submission.Id ?? 0,
            AuthorId = submission.AuthorId ?? 0,
            Compiler = submission.Compiler,
            Memory = submission.Memory ?? 0,
            Time = TimeSpan.FromMilliseconds(submission.Time ?? 0)
                           .ToDuration(),
            ProblemAlias = submission.ProblemAlias,
            ProblemId = submission.ProblemId,
            SubmissionTime = DateTimeOffset.TryParse(submission.SubmissionTime,
                                                     out var submissionTime)
                                 ? submissionTime.ToTimestamp()
                                 : null,
            TimeFromStart = TimeSpan.FromMilliseconds(submission.TimeFromStart ?? 0)
                                    .ToDuration(),
            Score = submission.Score is { } score
                        ? Convert.ToDecimal(score)
                        : null,
            Test = submission.Test,
            Verdict = submission.Verdict
        };
}
