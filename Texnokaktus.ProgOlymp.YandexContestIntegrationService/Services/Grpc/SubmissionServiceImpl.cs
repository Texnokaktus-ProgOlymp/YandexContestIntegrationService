using Amazon.S3.Transfer;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware.Options;
using Microsoft.Net.Http.Headers;
using MimeKit;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using YandexContestClient.Client;
using YandexContestClient.Client.Models;
using Enum = System.Enum;
using Submission = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.Submission;
using TestLog = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.TestLog;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

public class SubmissionServiceImpl(ContestClient client, ITransferUtility transferUtility) : SubmissionService.SubmissionServiceBase
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

    public override async Task<SubmissionFullReport> GetSubmissionFullReport(GetSubmissionFullReportRequest request, ServerCallContext context)
    {
        var runReport = await client.Contests[request.ContestStageId]
                                    .Submissions[request.SubmissionId]
                                    .Full
                                    .GetAsync(cancellationToken: context.CancellationToken);

        return new()
        {
            IpAddress = runReport?.Ip,
            Tests =
            {
                runReport?.CheckerLog?.Select(log => log.MapTestLog())
            }
        };
    }

    public override async Task<DownloadSubmissionSourceResponse> DownloadSubmissionSource(DownloadSubmissionSourceRequest request, ServerCallContext context)
    {
        var headersInspectionHandlerOption = new HeadersInspectionHandlerOption
        {
            InspectResponseHeaders = true
        };

        await using var stream = await client.Contests[request.ContestStageId]
                                             .Submissions[request.SubmissionId]
                                             .Source
                                             .GetAsync(configuration => configuration.Options.Add(headersInspectionHandlerOption),
                                                       context.CancellationToken);

        var key = headersInspectionHandlerOption.ResponseHeaders.TryGetValue(HeaderNames.ContentDisposition, out var contentDispositionValues)
               && contentDispositionValues.FirstOrDefault() is { } contentDispositionValue
               && ContentDisposition.TryParse(contentDispositionValue, out var contentDisposition)
               && contentDisposition.FileName is { } fileName
                      ? fileName
                      : request.SubmissionId.ToString();

        await transferUtility.UploadAsync(new()
                                          {
                                              BucketName = "submissions",
                                              Key = key,
                                              InputStream = stream
                                          },
                                          context.CancellationToken);

        return new()
        {
            FileName = key
        };
    }

    public override async Task<Empty> RejudgeSubmission(RejudgeSubmissionRequest request, ServerCallContext context)
    {
        await client.Submissions[request.SubmissionId].Rejudge.PostAsync();
        return new();
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
            Verdict = submission.Verdict.MapVerdict()
        };

    public static TestLog MapTestLog(this YandexContestClient.Client.Models.TestLog testLog) =>
        new()
        {
            SequenceNumber = testLog.SequenceNumber ?? 0,
            TestName = testLog.TestName,
            TestSetId = testLog.TestsetIdx ?? 0,
            Memory = testLog.MemoryUsed,
            Time = TimeSpan.FromMilliseconds(testLog.RunningTime ?? 0).ToDuration(),
            IsSample = testLog.IsSample ?? false,
            Verdict = testLog.Verdict.MapVerdict()
        };

    private static Verdict MapVerdict(this string? verdictString) =>
        string.IsNullOrWhiteSpace(verdictString)
            ? Verdict.Pending
            : Enum.TryParse(typeof(Verdict), verdictString, true, out var result)
           && result is Verdict verdict
                ? verdict
                : Verdict.Unknown;
}
