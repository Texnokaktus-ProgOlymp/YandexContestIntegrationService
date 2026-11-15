using System.Text.Json;
using Grpc.Core;
using Microsoft.AspNetCore.WebUtilities;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests;

[TestFixture]
public sealed class SubmissionPaginationTests : SetupBase
{
    [Test]
    public async Task PagesRemainIndependentlyAddressableAcrossEmptyPagesAndDuplicateIds()
    {
        Factory.Upstream.EnqueueJson(
            """{"count":2,"submissions":[{"id":417,"authorId":901,"problemId":"A"},{"id":417,"authorId":901,"problemId":"A"}]}"""
        );

        Factory.Upstream.EnqueueJson("""{"count":0,"submissions":[]}""");
        Factory.Upstream.EnqueueJson("""{"count":1,"submissions":[{"id":419,"authorId":902,"problemId":"B"}]}""");
        var first = await Page(1);
        var empty = await Page(2);
        var third = await Page(3);

        Assert.Multiple(() =>
            {
                Assert.That(
                    first.Submissions.Select(submission => submission.Id),
                    Is.EqualTo(
                        new long[]
                        {
                            417,
                            417
                        }
                    )
                );

                Assert.That(first.UpstreamPageCount, Is.EqualTo(2));
                Assert.That(empty.Submissions, Is.Empty);
                Assert.That(empty.HasUpstreamPageCount, Is.True);
                Assert.That(empty.UpstreamPageCount, Is.Zero);
                Assert.That(third.Submissions.Single().Id, Is.EqualTo(419));

                Assert.That(
                    new[]
                    {
                        first.PageIndex,
                        empty.PageIndex,
                        third.PageIndex
                    },
                    Is.EqualTo(
                        new[]
                        {
                            1,
                            2,
                            3
                        }
                    )
                );

                Assert.That(
                    new[]
                    {
                        first.PageSize,
                        empty.PageSize,
                        third.PageSize
                    },
                    Is.All.EqualTo(2)
                );

                Assert.That(
                    Factory.Upstream.Requests.Select(request => QueryHelpers.ParseQuery(request.Uri.Query)["page"]
                                                                            .ToString()
                    ),
                    Is.EqualTo(
                        new[]
                        {
                            "1",
                            "2",
                            "3"
                        }
                    )
                );

                Assert.That(
                    Factory.Upstream.Requests.All(request => QueryHelpers.ParseQuery(request.Uri.Query)["pageSize"]
                                                          == "2"
                    ),
                    Is.True
                );
            }
        );
    }

    [Test]
    public async Task AbsentUpstreamCountDoesNotInventATotal()
    {
        Factory.Upstream.EnqueueJson("""{"submissions":[]}""");
        var response = await Page(3);
        Assert.That(response.HasUpstreamPageCount, Is.False);
        Assert.That(response.PageIndex, Is.EqualTo(3));
    }

    [TestCase("", false, null)]
    [TestCase(",\"score\":null", false, null)]
    [TestCase(",\"score\":0", true, "0")]
    [TestCase(",\"score\":1.005", true, "1.005")]
    [TestCase(",\"score\":1.00500000000000000001", true, "1.00500000000000000001")]
    public async Task RawNumericScoreRetainsPrecisionBeyondTheLegacyDouble(string field, bool present, string? text)
    {
        Factory.Upstream.EnqueueJson($$"""{"count":1,"submissions":[{"id":417,"authorId":901{{field}}}]}""");
        var response = await Page(1);
        var observation = response.Submissions.Single().Observation;

        Assert.Multiple(() =>
            {
                Assert.That(observation.HasScoreText, Is.EqualTo(present));

                if (present)
                    Assert.That(observation.ScoreText, Is.EqualTo(text));

                Assert.That(observation.Id, Is.EqualTo(417));
                Assert.That(observation.AuthorId, Is.EqualTo(901));
                Assert.That(Factory.Upstream.Requests.Count, Is.EqualTo(1));
            }
        );

        using var raw = JsonDocument.Parse(observation.RawJson);

        if (present)
            Assert.That(raw.RootElement.GetProperty("score").GetRawText(), Is.EqualTo(text));
    }

    [Test]
    public async Task CancellationReachesTheInFlightUpstreamRequest()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancelled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Factory.Upstream.Enqueue(async (_, token) =>
            {
                entered.TrySetResult();

                try
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, token);

                    throw new InvalidOperationException("Unreachable");
                }
                catch (OperationCanceledException)
                {
                    cancelled.TrySetResult();

                    throw;
                }
            }
        );

        using var cancellation = new CancellationTokenSource();

        using var call = Api.Unary(
            SubmissionServiceName,
            "GetSubmissionsPage",
            new GetSubmissionsPageRequest
            {
                ContestStageId = 401,
                PageIndex = 3,
                PageSize = 2
            },
            GetSubmissionsPageResponse.Parser,
            cancellation.Token
        );

        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cancellation.Cancel();
        var failure = Assert.ThrowsAsync<RpcException>(async () => await call.ResponseAsync);
        await cancelled.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.That(failure, Is.Not.Null);
        Assert.That(Factory.Upstream.Requests.Count, Is.EqualTo(1));
    }

    private async Task<GetSubmissionsPageResponse> Page(int index) => await Api.Unary(
                                                                                    SubmissionServiceName,
                                                                                    "GetSubmissionsPage",
                                                                                    new GetSubmissionsPageRequest
                                                                                    {
                                                                                        ContestStageId = 401,
                                                                                        PageIndex = index,
                                                                                        PageSize = 2
                                                                                    },
                                                                                    GetSubmissionsPageResponse.Parser
                                                                                )
                                                                               .ResponseAsync;
}
