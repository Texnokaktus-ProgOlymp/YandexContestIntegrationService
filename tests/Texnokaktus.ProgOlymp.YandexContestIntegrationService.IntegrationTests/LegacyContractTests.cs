using Google.Protobuf;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests;

[TestFixture]
public sealed class LegacyContractTests : SetupBase
{
    [Test]
    public async Task LegacyBinaryRequestStillCallsTheExistingFullReportRpcAndOldReaderSkipsObservation()
    {
        // Captured old tags: contestStageId=1, submissionId=2; report ipAddress=1, tests=2.
        var request = GetSubmissionFullReportRequest.Parser.ParseFrom(Convert.FromHexString("08910310A103"));

        Factory.Upstream.EnqueueJson(
            """
            {"ip":"192.0.2.1","checkerLog":[{"sequenceNumber":1,"testName":"sample","testsetIdx":2,"verdict":"OK"}],
             "contestId":401,"runId":417,"status":"FINISHED","finalScore":"1.005"}
            """
        );

        var response = await Api.Unary(
                                     SubmissionServiceName,
                                     "GetSubmissionFullReport",
                                     request,
                                     SubmissionFullReport.Parser
                                 )
                                .ResponseAsync;

        using var oldReader = new CodedInputStream(response.ToByteArray());
        string? ip = null;
        var tests = new List<TestLog>();
        uint tag;

        while ((tag = oldReader.ReadTag()) != 0)
        {
            switch (tag)
            {
                case 10: ip = oldReader.ReadString(); break;
                case 18: tests.Add(TestLog.Parser.ParseFrom(oldReader.ReadBytes())); break;
                default: oldReader.SkipLastField(); break;
            }
        }

        Assert.Multiple(() =>
            {
                Assert.That(request.ContestStageId, Is.EqualTo(401));
                Assert.That(request.SubmissionId, Is.EqualTo(417));
                Assert.That(request.ToByteArray(), Is.EqualTo(Convert.FromHexString("08910310A103")));
                Assert.That(ip, Is.EqualTo("192.0.2.1"));
                Assert.That(tests.Single().TestName, Is.EqualTo("sample"));
                Assert.That(tests.Single().SequenceNumber, Is.EqualTo(1));
                Assert.That(tests.Single().TestSetId, Is.EqualTo(2));
                Assert.That(tests.Single().Verdict, Is.EqualTo(Verdict.Ok));
                Assert.That(response.Observation.FinalScoreText, Is.EqualTo("1.005"));
            }
        );
    }

    [Test]
    public async Task OriginalSubmissionStreamingRpcRemainsCallableWithItsLegacyRequest()
    {
        Factory.Upstream.EnqueueJson(
            """
            {"count":1,"submissions":[{"id":417,"authorId":901,"compiler":"gcc","problemAlias":"A","problemId":"problem-42","score":1.005,"verdict":"OK"}]}
            """
        );

        var request = GetSubmissionsRequest.Parser.ParseFrom(Convert.FromHexString("089103"));
        using var call = Api.Stream(SubmissionServiceName, "GetSubmissions", request, Submission.Parser);
        Assert.That(await call.ResponseStream.MoveNext(CancellationToken.None), Is.True);
        var submission = call.ResponseStream.Current;
        Assert.That(await call.ResponseStream.MoveNext(CancellationToken.None), Is.False);

        Assert.Multiple(() =>
            {
                Assert.That(submission.Id, Is.EqualTo(417));
                Assert.That(submission.AuthorId, Is.EqualTo(901));
                Assert.That(submission.Compiler, Is.EqualTo("gcc"));
                Assert.That(submission.ProblemAlias, Is.EqualTo("A"));
                Assert.That(submission.ProblemId, Is.EqualTo("problem-42"));
                Assert.That(submission.Verdict, Is.EqualTo(Verdict.Ok));
            }
        );
    }

    [Test]
    public void HistoricalReportAndArchiveBinaryFieldsRemainReadable()
    {
        var report = SubmissionFullReport.Parser.ParseFrom(
            Convert.FromHexString("0A093139322E302E322E31120C0801120673616D706C653801")
        );

        var archive = DownloadSubmissionSourceResponse.Parser.ParseFrom(Convert.FromHexString("0A0772756E2E637070"));

        Assert.Multiple(() =>
            {
                Assert.That(report.IpAddress, Is.EqualTo("192.0.2.1"));
                Assert.That(report.Tests.Single().TestName, Is.EqualTo("sample"));
                Assert.That(report.Tests.Single().Verdict, Is.EqualTo(Verdict.Ok));
                Assert.That(report.Observation, Is.Null);
                Assert.That(archive.FileName, Is.EqualTo("run.cpp"));
            }
        );
    }
}
