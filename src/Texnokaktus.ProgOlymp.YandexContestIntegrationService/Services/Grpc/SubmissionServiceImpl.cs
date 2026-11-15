using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Kiota.Abstractions;
using Amazon.S3.Transfer;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware.Options;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using YandexContestClient.Client;
using YandexContestClient.Client.Models;
using Enum = System.Enum;
using Submission = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.Submission;
using TestLog = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.TestLog;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

/// <summary>
/// A service class implementing the gRPC-based submission handling functionality, providing various operations
/// related to submissions in the Yandex Contest.
/// </summary>
public class SubmissionServiceImpl(ContestClient client, ITransferUtility transferUtility)
    : SubmissionService.SubmissionServiceBase
{
    /// <summary>
    /// Retrieves the list of submissions for a specific contest stage and streams them to the response stream.
    /// </summary>
    /// <param name="request">The request object containing details such as the ContestStageId for which submissions need to be fetched.</param>
    /// <param name="responseStream">The server stream writer used to send the submissions back to the client in a streamed fashion.</param>
    /// <param name="context">The context of the server call, used to monitor for cancellation or access other gRPC metadata.</param>
    /// <returns>A Task that represents the asynchronous operation of streaming submissions to the response stream.</returns>
    public override async Task GetSubmissions(
        GetSubmissionsRequest request,
        IServerStreamWriter<Submission> responseStream,
        ServerCallContext context
    )
    {
        var currentPage = 0;
        var processed = 0;

        Submissions? submissions;

        do
        {
            submissions = await client.V2
                                      .Contests[request.ContestStageId]
                                      .Submissions
                                      .GetAsync(
                                           configuration => configuration.QueryParameters.Page = ++currentPage,
                                           context.CancellationToken
                                       );

            foreach (var submission in submissions?.SubmissionsProp ?? [])
            {
                await responseStream.WriteAsync(submission.MapSubmission(), context.CancellationToken);
                processed++;
            }
        } while (processed < submissions?.Count);
    }

    /// <summary>
    /// Retrieves a detailed full report of a specific submission for a given contest stage.
    /// </summary>
    /// <param name="request">The request object containing the ContestStageId and SubmissionId to identify the specific submission.</param>
    /// <param name="context">The context of the server call, providing cancellation tokens and metadata related to the gRPC request.</param>
    /// <returns>A Task that represents the asynchronous operation, returning a detailed SubmissionFullReport object containing the IP address and test details of the submission.</returns>
    public override async Task<SubmissionFullReport> GetSubmissionFullReport(
        GetSubmissionFullReportRequest request,
        ServerCallContext context
    )
    {
        var handler = new NativeResponseHandler();

        await client.V2.Contests[request.ContestStageId]
                    .Submissions[request.SubmissionId]
                    .Full
                    .GetAsync(
                         configuration =>
                             configuration.Options.Add(new ResponseHandlerOption { ResponseHandler = handler }),
                         context.CancellationToken
                     );

        var (report, json) = await ContestObservationReader.ReadAsync(
                                 handler,
                                 FullRunReport.CreateFromDiscriminatorValue,
                                 context.CancellationToken
                             );

        if (report.ContestId is { } contestId && contestId != request.ContestStageId
         || report.RunId is { } runId && runId != request.SubmissionId)
            throw new RpcException(new(StatusCode.DataLoss, "Contest returned a different report identity."));

        using var document = JsonDocument.Parse(json);

        return new()
        {
            IpAddress = report.Ip ?? string.Empty,
            Tests = { report.CheckerLog?.Select(log => log.MapTestLog()) ?? [] },
            Observation = report.MapObservation(document.RootElement)
        };
    }

    /// <summary>
    /// Retrieves exactly the requested page, without interpreting its count as a global total.
    /// </summary>
    public override async Task<GetSubmissionsPageResponse> GetSubmissionsPage(
        GetSubmissionsPageRequest request,
        ServerCallContext context
    )
    {
        if (request.PageIndex < 1 || request.PageSize < 1)
            throw new RpcException(new(StatusCode.InvalidArgument, "Page index and size must be positive."));

        var handler = new NativeResponseHandler();

        await client.V2.Contests[request.ContestStageId]
                    .Submissions.GetAsync(
                         configuration =>
                         {
                             configuration.QueryParameters.Page = request.PageIndex;
                             configuration.QueryParameters.PageSize = request.PageSize;
                             configuration.Options.Add(new ResponseHandlerOption { ResponseHandler = handler });
                         },
                         context.CancellationToken
                     );

        var (page, json) = await ContestObservationReader.ReadAsync(
                               handler,
                               Submissions.CreateFromDiscriminatorValue,
                               context.CancellationToken
                           );

        using var document = JsonDocument.Parse(json);
        var items = page.SubmissionsProp ?? [];

        var rawItems = document.RootElement.TryGetProperty("submissions", out var values)
                    && values.ValueKind == JsonValueKind.Array
                           ? values.EnumerateArray().ToArray()
                           : [];

        if (items.Count != rawItems.Length)
            throw new RpcException(new(StatusCode.DataLoss, "Contest returned inconsistent page items."));

        var response = new GetSubmissionsPageResponse
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };

        if (page.Count is { } count) response.UpstreamPageCount = count;

        for (var index = 0; index < items.Count; index++)
            response.Submissions.Add(items[index].MapObservedSubmission(rawItems[index]));

        return response;
    }

    /// <summary>
    /// Downloads the source code of a specific submission from Yandex.Contest and uploads it to the specified storage service.
    /// </summary>
    /// <param name="request">The request object containing the ContestStageId and SubmissionId for which the source needs to be fetched.</param>
    /// <param name="context">The server call context, used to manage metadata and monitor for cancellation.</param>
    /// <returns>A task returning the actual uploaded object key, independent of the upstream file name.</returns>
    public override async Task<DownloadSubmissionSourceResponse> DownloadSubmissionSource(
        DownloadSubmissionSourceRequest request,
        ServerCallContext context
    )
    {
        var handler = new NativeResponseHandler();

        await client.V2.Contests[request.ContestStageId]
                    .Submissions[request.SubmissionId]
                    .Source.GetAsync(
                         configuration =>
                             configuration.Options.Add(new ResponseHandlerOption { ResponseHandler = handler }),
                         context.CancellationToken
                     );

        using var response = handler.Value as HttpResponseMessage
                          ?? throw new RpcException(
                                 new(StatusCode.Unavailable, "Contest returned no source response.")
                             );

        if (!response.IsSuccessStatusCode)
            throw new RpcException(
                new(
                    response.StatusCode switch
                    {
                        HttpStatusCode.Unauthorized           => StatusCode.Unauthenticated,
                        HttpStatusCode.Forbidden              => StatusCode.PermissionDenied,
                        HttpStatusCode.NotFound               => StatusCode.NotFound,
                        HttpStatusCode.TooManyRequests        => StatusCode.ResourceExhausted,
                        >= HttpStatusCode.InternalServerError => StatusCode.Unavailable,
                        _                                     => StatusCode.FailedPrecondition
                    },
                    "Contest rejected the source request."
                )
            );

        await using var stream = await response.Content.ReadAsStreamAsync(context.CancellationToken);

        var upload = new TransferUtilityUploadRequest
        {
            BucketName = "submissions",
            Key =
                FormattableString.Invariant(
                    $"contests/{request.ContestStageId}/submissions/{request.SubmissionId}/source"
                ),
            InputStream = stream,
            AutoCloseStream = false,
            ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream"
        };

        upload.Headers.ContentDisposition = response.Content.Headers.ContentDisposition?.ToString()
                                         ?? new ContentDispositionHeaderValue("attachment")
                                            {
                                                FileName = request.SubmissionId.ToString(
                                                    CultureInfo.InvariantCulture
                                                )
                                            }.ToString();

        await transferUtility.UploadAsync(upload, context.CancellationToken);

        return new() { FileName = upload.Key };
    }

    /// <summary>
    /// Triggers a rejudging operation for a specific submission identified by its SubmissionId.
    /// </summary>
    /// <param name="request">The request object containing the SubmissionId of the submission to be rejudged.</param>
    /// <param name="context">The context of the server call, providing metadata and monitoring for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous operation, which completes when the rejudging request has been successfully initiated.</returns>
    public override async Task<Empty> RejudgeSubmission(RejudgeSubmissionRequest request, ServerCallContext context)
    {
        await client.V2.Submissions[request.SubmissionId].Rejudge.PostAsync();

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
            Compiler = submission.Compiler ?? string.Empty,
            Memory = submission.Memory ?? 0,
            Time = TimeSpan.FromMilliseconds(submission.Time ?? 0)
                           .ToDuration(),
            ProblemAlias = submission.ProblemAlias ?? string.Empty,
            ProblemId = submission.ProblemId ?? string.Empty,
            SubmissionTime = DateTimeOffset.TryParse(
                                 submission.SubmissionTime,
                                 out var submissionTime
                             )
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

    public static Submission MapObservedSubmission(
        this YandexContestClient.Client.Models.Submission source,
        JsonElement raw
    )
    {
        var result = source.MapSubmission();

        var observation = new Submission.Types.Observation
        {
            SubmissionTime = ContestObservationReader.Timestamp(source.SubmissionTime),
            TimeFromStart = ContestObservationReader.Milliseconds(source.TimeFromStart),
            RawJson = ContestObservationReader.TechnicalJson(
                raw,
                "id",
                "authorId",
                "compiler",
                "memory",
                "time",
                "problemAlias",
                "problemId",
                "submissionTime",
                "timeFromStart",
                "score",
                "test",
                "verdict"
            )
        };

        if (ContestObservationReader.Int64(raw, "id") is { } id) observation.Id = id;
        if (ContestObservationReader.Int64(raw, "authorId") is { } authorId) observation.AuthorId = authorId;
        if (ContestObservationReader.Text(raw, "score") is { } score) observation.ScoreText = score;
        if (ContestObservationReader.Text(raw, "verdict") is { } verdict) observation.VerdictText = verdict;
        result.Observation = observation;

        return result;
    }

    public static SubmissionFullReport.Types.Observation MapObservation(this FullRunReport source, JsonElement raw)
    {
        var identity = new SubmissionFullReport.Types.Observation.Types.Identity();
        if (ContestObservationReader.Int64(raw, "contestId") is { } contestId) identity.ContestId = contestId;
        if (ContestObservationReader.Int64(raw, "runId") is { } runId) identity.RunId = runId;

        if (raw.TryGetProperty("participantInfo", out var participant) && participant.ValueKind != JsonValueKind.Null)
        {
            if (participant.ValueKind != JsonValueKind.Object)
                throw new RpcException(new(StatusCode.DataLoss, "Contest returned an invalid participant identity."));

            if (ContestObservationReader.Int64(participant, "id") is { } participantId)
                identity.ParticipantId = participantId;
        }

        if (source.ProblemId is { } problemId) identity.ProblemId = problemId;

        var result = new SubmissionFullReport.Types.Observation
        {
            Identity = identity,
            ProcessingState = source.Status switch
            {
                FullRunReport_status.WAITING => SubmissionFullReport.Types.Observation.Types.ProcessingState.Waiting,
                FullRunReport_status.RUNNING => SubmissionFullReport.Types.Observation.Types.ProcessingState.Running,
                FullRunReport_status.FINISHED => SubmissionFullReport.Types.Observation.Types.ProcessingState.Finished,
                FullRunReport_status.FAILED => SubmissionFullReport.Types.Observation.Types.ProcessingState.Failed,
                _ => SubmissionFullReport.Types.Observation.Types.ProcessingState.Unspecified
            },
            SubmissionTime = ContestObservationReader.Timestamp(source.SubmissionTime),
            TimeFromStart = ContestObservationReader.Milliseconds(source.TimeFromStart),
            RawJson = ContestObservationReader.TechnicalJson(
                raw,
                "contestId",
                "runId",
                "participantInfo",
                "problemId",
                "problemAlias",
                "status",
                "verdict",
                "finalScore",
                "preliminaryScore",
                "submissionTime",
                "timeFromStart",
                "globalError"
            )
        };

        if (ContestObservationReader.Text(raw, "status") is { } status) result.ProcessingStateText = status;
        if (ContestObservationReader.Text(raw, "verdict") is { } verdict) result.VerdictText = verdict;
        if (ContestObservationReader.Text(raw, "finalScore") is { } finalScore) result.FinalScoreText = finalScore;

        if (ContestObservationReader.Text(raw, "preliminaryScore") is { } preliminaryScore)
            result.PreliminaryScoreText = preliminaryScore;

        if (ContestObservationReader.Text(raw, "globalError") is { } globalError) result.GlobalErrorText = globalError;

        return result;
    }

    public static TestLog MapTestLog(this YandexContestClient.Client.Models.TestLog testLog) =>
        new()
        {
            SequenceNumber = testLog.SequenceNumber ?? 0,
            TestName = testLog.TestName ?? string.Empty,
            TestSetId = testLog.TestsetIdx ?? 0,
            Memory = testLog.MemoryUsed,
            Time = ContestObservationReader.Milliseconds(testLog.RunningTime),
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
