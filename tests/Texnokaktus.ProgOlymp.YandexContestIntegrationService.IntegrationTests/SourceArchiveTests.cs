using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests;

[TestFixture]
public sealed class SourceArchiveTests : SetupBase
{
    [TestCase("solutions/contest-401/run-417.cpp", "solutions/contest-401/run-417.cpp")]
    [TestCase(null, "417")]
    public async Task ReturnedFileNameIsTheExactActualS3ObjectKey(string? upstreamFileName, string expectedKey)
    {
        Factory.Upstream.EnqueueSource("int main() { return 0; }", upstreamFileName);

        var response = await Api.Unary(
                                     SubmissionServiceName,
                                     "DownloadSubmissionSource",
                                     new DownloadSubmissionSourceRequest
                                     {
                                         ContestStageId = 401,
                                         SubmissionId = 417
                                     },
                                     DownloadSubmissionSourceResponse.Parser
                                 )
                                .ResponseAsync;

        var uploaded = Factory.Storage.Uploads.Single();

        Assert.Multiple(() =>
            {
                Assert.That(uploaded.Bucket, Is.EqualTo("submissions"));
                Assert.That(uploaded.Key, Is.EqualTo(expectedKey));
                Assert.That(response.FileName, Is.EqualTo(uploaded.Key));
                Assert.That(uploaded.Body, Is.EqualTo("int main() { return 0; }"));
                Assert.That(Factory.Upstream.Requests.Count, Is.EqualTo(1));
            }
        );
    }

    [Test]
    public async Task FailedS3UploadCannotReportAnArchiveKeyAsSuccess()
    {
        Factory.Storage.FailUploads = true;
        Factory.Upstream.EnqueueSource("source", "failed.cpp");

        using var call = Api.Unary(
            SubmissionServiceName,
            "DownloadSubmissionSource",
            new DownloadSubmissionSourceRequest
            {
                ContestStageId = 401,
                SubmissionId = 417
            },
            DownloadSubmissionSourceResponse.Parser
        );

        Assert.ThrowsAsync<RpcException>(async () => await call.ResponseAsync);
        Assert.That(Factory.Storage.Uploads.Single().Key, Is.EqualTo("failed.cpp"));
    }
}
