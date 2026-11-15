using System.Net;
using System.Net.Http.Headers;
using Google.Protobuf;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests.Tools;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests;

[TestFixture]
public sealed class SourceArchiveTests : SetupBase
{
    [TestCase("attachment; filename=main.cpp", "main.cpp", "text/x-c++src; charset=utf-8")]
    [TestCase(null, "417", null)]
    [TestCase(
        "attachment; filename*=utf-8''%D1%80%D0%B5%D1%88%D0%B5%D0%BD%D0%B8%D0%B5.cpp",
        "решение.cpp",
        "application/octet-stream"
    )]
    public async Task BinaryBytesAndAvailableHeadersAreArchivedUnderTheActualReturnedKey(
        string? disposition,
        string expectedFileName,
        string? contentType
    )
    {
        byte[] bytes = [0, 255, 254, 65, 0, 13, 10, 128];
        var content = new ByteArrayContent(bytes);

        if (disposition is not null)
            content.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse(disposition);

        if (contentType is not null) content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        Enqueue(content);
        var response = await Archive();
        var uploaded = Factory.Storage.Uploads.Single();
        var storedDisposition = ContentDispositionHeaderValue.Parse(uploaded.ContentDisposition!);
        using var oldReader = new CodedInputStream(response.ToByteArray());

        Assert.Multiple(() =>
            {
                Assert.That(uploaded.Bucket, Is.EqualTo("submissions"));
                Assert.That(uploaded.Key, Is.EqualTo("contests/401/submissions/417/source"));
                Assert.That(response.FileName, Is.EqualTo(uploaded.Key));
                Assert.That(uploaded.Body, Is.EqualTo(bytes));
                Assert.That(uploaded.ContentType, Is.EqualTo(contentType ?? "application/octet-stream"));

                Assert.That(
                    storedDisposition.FileNameStar ?? storedDisposition.FileName?.Trim('"'),
                    Is.EqualTo(expectedFileName)
                );

                Assert.That(oldReader.ReadTag(), Is.EqualTo(10));
                Assert.That(oldReader.ReadString(), Is.EqualTo(uploaded.Key));
                Assert.That(oldReader.ReadTag(), Is.Zero);
                Assert.That(Factory.Upstream.Requests.Count, Is.EqualTo(1));
            }
        );
    }

    [Test]
    public async Task IdenticalFileNamesCannotCollideAcrossRunsOrContestsAndSameRunRetriesKeepTheirKey()
    {
        var requests = new (long Contest, long Run, string Source)[]
        {
            (401, 417, "first"),
            (401, 418, "second"),
            (401, 417, "first"),
            (402, 417, "third")
        };

        var keys = new List<string>();

        foreach (var request in requests)
        {
            Factory.Upstream.EnqueueSource(request.Source, "main.cpp");
            keys.Add((await Archive(request.Contest, request.Run)).FileName);
        }

        Assert.Multiple(() =>
            {
                Assert.That(
                    keys,
                    Is.EqualTo(
                        new[]
                        {
                            "contests/401/submissions/417/source",
                            "contests/401/submissions/418/source",
                            "contests/401/submissions/417/source",
                            "contests/402/submissions/417/source"
                        }
                    )
                );

                Assert.That(Factory.Storage.Uploads.Select(upload => upload.Key), Is.EqualTo(keys));

                Assert.That(
                    Factory.Storage.Uploads.Select(upload => System.Text.Encoding.UTF8.GetString(upload.Body)),
                    Is.EqualTo(requests.Select(request => request.Source))
                );
            }
        );
    }

    [Test]
    public async Task FailedS3UploadCannotReportAnArchiveKeyAsSuccess()
    {
        Factory.Storage.FailUploads = true;
        var content = new TrackingContent("source");
        var stream = await content.ReadAsStreamAsync();
        Enqueue(content);
        Assert.ThrowsAsync<RpcException>(async () => await Archive());

        Assert.Multiple(() =>
            {
                Assert.That(Factory.Storage.Uploads.Single().Key, Is.EqualTo("contests/401/submissions/417/source"));
                Assert.That(content.IsDisposed, Is.True);
                Assert.That(stream.CanRead, Is.False);
            }
        );
    }

    [TestCase(401, StatusCode.Unauthenticated)]
    [TestCase(403, StatusCode.PermissionDenied)]
    [TestCase(404, StatusCode.NotFound)]
    [TestCase(503, StatusCode.Unavailable)]
    public void FailedUpstreamResponseIsDisposedWithoutArchivingItsErrorBody(int status, StatusCode expected)
    {
        var content = new TrackingContent("Controlled upstream error, not a source");
        Enqueue(content, (HttpStatusCode)status);
        var failure = Assert.ThrowsAsync<RpcException>(async () => await Archive());

        Assert.Multiple(() =>
            {
                Assert.That(failure!.StatusCode, Is.EqualTo(expected));
                Assert.That(Factory.Storage.Uploads, Is.Empty);
                Assert.That(content.IsDisposed, Is.True);
            }
        );
    }

    [Test]
    public async Task ResponseAndStreamStayAliveUntilUploadCompletesAndAreThenDisposed()
    {
        var content = new TrackingContent("source");
        var stream = await content.ReadAsStreamAsync();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var complete = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var readableDuringUpload = false;

        Factory.Storage.BeforeUploadComplete = async _ =>
        {
            readableDuringUpload = stream.CanRead && !content.IsDisposed;
            entered.TrySetResult();
            await complete.Task;
        };

        Enqueue(content);
        var download = Archive();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));

        try
        {
            Assert.That(readableDuringUpload, Is.True);
            Assert.That(download.IsCompleted, Is.False);
        }
        finally
        {
            complete.TrySetResult();
        }

        await download;

        Assert.Multiple(() =>
            {
                Assert.That(content.IsDisposed, Is.True);
                Assert.That(stream.CanRead, Is.False);
            }
        );
    }

    [Test]
    public async Task RpcCancellationReachesUploadAndDisposesTheUpstreamResponse()
    {
        var content = new TrackingContent("source");
        var stream = await content.ReadAsStreamAsync();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancelled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Factory.Storage.BeforeUploadComplete = async token =>
        {
            entered.TrySetResult();

            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
            }
            catch (OperationCanceledException)
            {
                cancelled.TrySetResult();

                throw;
            }
        };

        Enqueue(content);
        using var cancellation = new CancellationTokenSource();
        var download = Archive(token: cancellation.Token);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cancellation.Cancel();
        Assert.ThrowsAsync<RpcException>(async () => await download);
        await cancelled.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.That(() => content.IsDisposed, Is.True.After(2000, 20));
        Assert.That(stream.CanRead, Is.False);
    }

    [Test]
    public async Task CancellationWhileReadingTheSourceBodyDisposesItWithoutUploading()
    {
        var content = new BlockingContent();
        Enqueue(content);
        using var cancellation = new CancellationTokenSource();
        var download = Archive(token: cancellation.Token);
        await content.Entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cancellation.Cancel();
        Assert.ThrowsAsync<RpcException>(async () => await download);
        await content.Cancelled.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.That(() => content.IsDisposed, Is.True.After(2000, 20));
        Assert.That(Factory.Storage.Uploads, Is.Empty);
    }

    private void Enqueue(HttpContent content, HttpStatusCode status = HttpStatusCode.OK) =>
        Factory.Upstream.Enqueue((_, _) => Task.FromResult(new HttpResponseMessage(status) { Content = content }));

    private async Task<DownloadSubmissionSourceResponse> Archive(
        long contest = 401,
        long run = 417,
        CancellationToken token = default
    )
    {
        using var call = Api.Unary(
            SubmissionServiceName,
            "DownloadSubmissionSource",
            new DownloadSubmissionSourceRequest
            {
                ContestStageId = contest,
                SubmissionId = run
            },
            DownloadSubmissionSourceResponse.Parser,
            token
        );

        return await call.ResponseAsync;
    }
}
