using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests.Tools;
using Observation = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.SubmissionFullReport.Types.Observation;
using ProcessingState =
    Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest.SubmissionFullReport.Types.Observation.Types.
    ProcessingState;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests;

[TestFixture]
public sealed class SubmissionObservationTests : SetupBase
{
    [Test]
    public async Task WaitingZeroAndLaterFinishedAreSeparateObservationsOfTheSameRun()
    {
        Factory.Upstream.EnqueueJson(Report("WAITING", "0", "OK"));
        Factory.Upstream.EnqueueJson(Report("FINISHED", "1.005", "WRONG_ANSWER"));

        var waiting = await Read();
        var finished = await Read();

        Assert.Multiple(() =>
            {
                Assert.That(waiting.ProcessingState, Is.EqualTo(ProcessingState.Waiting));
                Assert.That(waiting.FinalScoreText, Is.EqualTo("0"));
                Assert.That(waiting.VerdictText, Is.EqualTo("OK"));
                Assert.That(finished.ProcessingState, Is.EqualTo(ProcessingState.Finished));
                Assert.That(finished.FinalScoreText, Is.EqualTo("1.005"));
                Assert.That(finished.VerdictText, Is.EqualTo("WRONG_ANSWER"));
                Assert.That(finished.Identity, Is.EqualTo(waiting.Identity));

                Assert.That(
                    Factory.Upstream.Requests.Count,
                    Is.EqualTo(2),
                    "One upstream response supplies both typed and raw fields."
                );
            }
        );
    }

    [Test]
    public async Task KnownRunCanBeRejudgedAndReturnToRunningBeforeFinishingAgain()
    {
        Factory.Upstream.EnqueueJson(Report("FINISHED", "100", "OK"));

        Factory.Upstream.Enqueue((_, _) => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NoContent))
        );

        Factory.Upstream.EnqueueJson(Report("RUNNING", "0", "OK"));
        Factory.Upstream.EnqueueJson(Report("FINISHED", "0", "WRONG_ANSWER"));

        var first = await Read();

        await Api.Unary(
                      SubmissionServiceName,
                      "RejudgeSubmission",
                      new RejudgeSubmissionRequest { SubmissionId = 417 },
                      Empty.Parser
                  )
                 .ResponseAsync;

        var running = await Read();
        var second = await Read();

        Assert.Multiple(() =>
            {
                Assert.That(first.ProcessingState, Is.EqualTo(ProcessingState.Finished));
                Assert.That(running.ProcessingState, Is.EqualTo(ProcessingState.Running));
                Assert.That(second.ProcessingState, Is.EqualTo(ProcessingState.Finished));
                Assert.That(second.Identity, Is.EqualTo(first.Identity));
                Assert.That(second.FinalScoreText, Is.EqualTo("0"));

                Assert.That(
                    Factory.Upstream.Requests.Select(request => request.Method),
                    Is.EqualTo(
                        new[]
                        {
                            HttpMethod.Get,
                            HttpMethod.Post,
                            HttpMethod.Get,
                            HttpMethod.Get
                        }
                    )
                );
            }
        );
    }

    [TestCase("WAITING", ProcessingState.Waiting)]
    [TestCase("RUNNING", ProcessingState.Running)]
    [TestCase("FINISHED", ProcessingState.Finished)]
    [TestCase("FAILED", ProcessingState.Failed)]
    [TestCase("FUTURE_STATE", ProcessingState.Unspecified)]
    public async Task ProcessingStateIsIndependentOfVerdictAndScore(string state, ProcessingState expected)
    {
        Factory.Upstream.EnqueueJson(Report(state, "100", "OK"));
        var observation = await Read();

        Assert.Multiple(() =>
            {
                Assert.That(observation.ProcessingState, Is.EqualTo(expected));
                Assert.That(observation.ProcessingStateText, Is.EqualTo(state));
                Assert.That(observation.VerdictText, Is.EqualTo("OK"));
            }
        );
    }

    [TestCase("{}", false, null)]
    [TestCase("{\"finalScore\":null}", false, null)]
    [TestCase("{\"finalScore\":\"\"}", true, "")]
    [TestCase("{\"finalScore\":\"0\"}", true, "0")]
    [TestCase("{\"finalScore\":\"1.005\"}", true, "1.005")]
    public async Task ScorePresenceAndRawJsonPreserveAbsentNullEmptyZeroAndExactText(
        string payload,
        bool present,
        string? score
    )
    {
        Factory.Upstream.EnqueueJson(payload);
        var observation = await Read();
        using var raw = JsonDocument.Parse(observation.RawJson);
        using var original = JsonDocument.Parse(payload);

        Assert.Multiple(() =>
            {
                Assert.That(observation.HasFinalScoreText, Is.EqualTo(present));

                if (present)
                    Assert.That(observation.FinalScoreText, Is.EqualTo(score));

                Assert.That(observation.ProcessingState, Is.EqualTo(ProcessingState.Unspecified));

                Assert.That(
                    raw.RootElement.TryGetProperty("finalScore", out var actual),
                    Is.EqualTo(original.RootElement.TryGetProperty("finalScore", out var expected))
                );

                if (original.RootElement.TryGetProperty("finalScore", out expected))
                    Assert.That(actual.GetRawText(), Is.EqualTo(expected.GetRawText()));
            }
        );
    }

    [Test]
    public async Task ActualIdentityAndTechnicalRawFieldsDoNotSubstituteTheRequestOrExposeSourceAndParticipantNames()
    {
        Factory.Upstream.EnqueueJson(
            """
            {"contestId":401,"runId":417,"participantInfo":{"id":901,"name":"private name","login":"private login"},
             "problemId":"problem-42","source":"private source","checkerLog":[],"status":"FAILED","globalError":"SYSTEM_ERROR"}
            """
        );

        var observation = await Read();
        using var raw = JsonDocument.Parse(observation.RawJson);

        Assert.Multiple(() =>
            {
                Assert.That(observation.Identity.ContestId, Is.EqualTo(401));
                Assert.That(observation.Identity.RunId, Is.EqualTo(417));
                Assert.That(observation.Identity.ParticipantId, Is.EqualTo(901));
                Assert.That(observation.Identity.ProblemId, Is.EqualTo("problem-42"));
                Assert.That(observation.RawJson, Does.Not.Contain("private"));
                Assert.That(raw.RootElement.TryGetProperty("source", out _), Is.False);
                Assert.That(raw.RootElement.TryGetProperty("checkerLog", out _), Is.False);
            }
        );
    }

    [Test]
    public async Task MissingActualIdentityRemainsAbsent()
    {
        Factory.Upstream.EnqueueJson("{}");
        var observation = await Read();

        Assert.Multiple(() =>
            {
                Assert.That(observation.Identity.HasContestId, Is.False);
                Assert.That(observation.Identity.HasRunId, Is.False);
                Assert.That(observation.Identity.HasParticipantId, Is.False);
                Assert.That(observation.Identity.HasProblemId, Is.False);
            }
        );
    }

    [TestCase("{\"contestId\":402,\"runId\":417}")]
    [TestCase("{\"contestId\":401,\"runId\":418}")]
    public async Task ConflictingActualIdentityFailsTheRpc(string payload)
    {
        Factory.Upstream.EnqueueJson(payload);
        var failure = Assert.ThrowsAsync<RpcException>(async () => await Read());
        Assert.That(failure!.StatusCode, Is.EqualTo(StatusCode.DataLoss));
        Assert.That(Factory.Upstream.Requests.Count, Is.EqualTo(1));
    }

    [TestCase("2026-11-15T00:04:05.678Z")]
    [TestCase("2026-11-15T03:04:05.678+03:00")]
    public async Task TimesPreserveUtcInstantAndMilliseconds(string timestamp)
    {
        Factory.Upstream.EnqueueJson($$"""{"submissionTime":"{{timestamp}}","timeFromStart":1234}""");
        var observation = await Read();

        Assert.Multiple(() =>
            {
                Assert.That(
                    observation.SubmissionTime.ToDateTimeOffset(),
                    Is.EqualTo(DateTimeOffset.Parse("2026-11-15T00:04:05.678Z"))
                );

                Assert.That(observation.TimeFromStart.ToTimeSpan(), Is.EqualTo(TimeSpan.FromMilliseconds(1234)));
            }
        );
    }

    [TestCase("{}")]
    [TestCase("{\"submissionTime\":null,\"timeFromStart\":null}")]
    public async Task MissingTimesRemainMissing(string payload)
    {
        Factory.Upstream.EnqueueJson(payload);
        var observation = await Read();

        Assert.Multiple(() =>
            {
                Assert.That(observation.SubmissionTime, Is.Null);
                Assert.That(observation.TimeFromStart, Is.Null);
            }
        );
    }

    [TestCase(401, StatusCode.Unauthenticated)]
    [TestCase(403, StatusCode.PermissionDenied)]
    [TestCase(404, StatusCode.NotFound)]
    [TestCase(429, StatusCode.ResourceExhausted)]
    [TestCase(503, StatusCode.Unavailable)]
    public void UpstreamHttpFailureCannotBecomeASuccessfulEmptyObservation(int status, StatusCode expected)
    {
        Factory.Upstream.Enqueue((_, _) => Task.FromResult(
                                     new HttpResponseMessage((System.Net.HttpStatusCode)status)
                                     {
                                         Content = new StringContent("{\"error\":\"controlled\"}")
                                     }
                                 )
        );

        var failure = Assert.ThrowsAsync<RpcException>(async () => await Read());
        Assert.That(failure!.StatusCode, Is.EqualTo(expected));
        Assert.That(Factory.Upstream.Requests.Count, Is.EqualTo(1));
    }

    [TestCase("{broken")]
    [TestCase("{\"runId\":\"not-an-integer\"}")]
    [TestCase("{\"contestId\":\"not-an-integer\"}")]
    [TestCase("{\"participantInfo\":{\"id\":\"not-an-integer\"}}")]
    [TestCase("{\"runId\":1.5}")]
    [TestCase("{\"runId\":9223372036854775808}")]
    public void MalformedSuccessPayloadFailsWithDataLoss(string payload)
    {
        Factory.Upstream.EnqueueJson(payload);
        var failure = Assert.ThrowsAsync<RpcException>(async () => await Read());
        Assert.That(failure!.StatusCode, Is.EqualTo(StatusCode.DataLoss));
    }

    [TestCase(200)]
    [TestCase(401)]
    public async Task NativeHttpResponseContentIsDisposedOnSuccessAndFailure(int status)
    {
        var content = new TrackingContent("{}");

        Factory.Upstream.Enqueue((_, _) => Task.FromResult(
                                     new HttpResponseMessage((System.Net.HttpStatusCode)status) { Content = content }
                                 )
        );

        if (status == 200)
            await Read();
        else
            Assert.ThrowsAsync<RpcException>(async () => await Read());

        Assert.That(content.IsDisposed, Is.True);
    }

    [Test]
    public async Task CancellationDuringReportBodyReadCancelsAndDisposesContent()
    {
        var content = new BlockingContent();

        Factory.Upstream.Enqueue((_, _) => Task.FromResult(
                                     new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = content }
                                 )
        );

        using var cancellation = new CancellationTokenSource();

        using var call = Api.Unary(
            SubmissionServiceName,
            "GetSubmissionFullReport",
            new GetSubmissionFullReportRequest
            {
                ContestStageId = 401,
                SubmissionId = 417
            },
            SubmissionFullReport.Parser,
            cancellation.Token
        );

        await content.Entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cancellation.Cancel();
        Assert.ThrowsAsync<RpcException>(async () => await call.ResponseAsync);
        await content.Cancelled.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.That(content.IsDisposed, Is.True);
    }

    private async Task<Observation> Read() => (await Api.Unary(
                                                             SubmissionServiceName,
                                                             "GetSubmissionFullReport",
                                                             new GetSubmissionFullReportRequest
                                                             {
                                                                 ContestStageId = 401,
                                                                 SubmissionId = 417
                                                             },
                                                             SubmissionFullReport.Parser
                                                         )
                                                        .ResponseAsync).Observation;

    private static string Report(string state, string score, string verdict) => $$"""
                                                                                  {"contestId":401,"runId":417,"participantInfo":{"id":901},"problemId":"problem-42",
                                                                                   "status":"{{state}}","finalScore":"{{score}}","verdict":"{{verdict}}","checkerLog":[],"ip":"192.0.2.1"}
                                                                                  """;
}
