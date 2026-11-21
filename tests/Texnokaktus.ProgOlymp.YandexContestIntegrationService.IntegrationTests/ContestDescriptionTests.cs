using System.Net;
using Google.Protobuf;
using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests;

[TestFixture]
public sealed class ContestDescriptionTests : SetupBase
{
    [TestCase("PROHIBITED", UpsolvingAllowance.Prohibited, 0)]
    [TestCase("ALLOWED_AFTER_PARTICIPATION_ENDS", UpsolvingAllowance.AllowedAfterParticipationEnds, 1)]
    [TestCase("ALLOWED_AFTER_CONTEST_ENDS", UpsolvingAllowance.AllowedAfterContestEnds, 2)]
    public async Task DescriptionOnlyPreservesObservedAllowanceWithoutCallingUnavailableOwner(
        string sourceValue,
        UpsolvingAllowance expected,
        int wireValue
    )
    {
        Factory.Upstream.EnqueueJson(Description($",\"upsolvingAllowance\":\"{sourceValue}\""));
        Factory.Upstream.Enqueue((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

        var response = await Read(
                           new()
                           {
                               ContestId = 401,
                               DescriptionOnly = true
                           }
                       );

        Assert.Multiple(() =>
            {
                Assert.That(response.Result, Is.Not.Null);
                Assert.That(response.Result.HasUpsolvingAllowance, Is.True);
                Assert.That(response.Result.UpsolvingAllowance, Is.EqualTo(expected));
                Assert.That(AllowanceOnWire(response.Result), Is.EqualTo(wireValue));
                Assert.That(response.Result.FinishTime, Is.Null);

                Assert.That(
                    Factory.Upstream.Requests.Select(request => request.Uri.AbsolutePath),
                    Is.EqualTo(new[] { "/api/public/v2/contests/401" })
                );
            }
        );
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task LegacyRequestAndExplicitFalseRetainOwnerFinishTime(bool explicitFalse)
    {
        var request = GetContestRequest.Parser.ParseFrom(
            explicitFalse
                ? new byte[]
                {
                    8,
                    42,
                    16,
                    0
                }
                : new byte[]
                {
                    8,
                    42
                }
        );

        Factory.Upstream.EnqueueJson(Description(",\"upsolvingAllowance\":\"PROHIBITED\""));
        Factory.Upstream.EnqueueJson("""{"contestFinishTime":"2026-11-01T11:00:00Z"}""");

        var response = await Read(request);

        Assert.Multiple(() =>
            {
                Assert.That(request.DescriptionOnly, Is.False);

                Assert.That(
                    response.Result.FinishTime.ToDateTimeOffset(),
                    Is.EqualTo(DateTimeOffset.Parse("2026-11-01T11:00:00Z"))
                );

                Assert.That(response.Result.HasUpsolvingAllowance, Is.True);

                Assert.That(
                    Factory.Upstream.Requests.Select(item => item.Uri.AbsolutePath),
                    Is.EqualTo(
                        new[]
                        {
                            "/api/public/v2/contests/42",
                            "/api/public/v2/contests/42/participation"
                        }
                    )
                );
            }
        );
    }

    [TestCase("")]
    [TestCase(",\"upsolvingAllowance\":null")]
    [TestCase(",\"upsolvingAllowance\":\"NOT_A_MODE\"")]
    public void MissingOrInvalidSourceAllowanceFailsRatherThanInventingProhibited(string field)
    {
        Factory.Upstream.EnqueueJson(Description(field));

        Assert.ThrowsAsync<RpcException>(async () =>
                                             await Read(
                                                 new()
                                                 {
                                                     ContestId = 401,
                                                     DescriptionOnly = true
                                                 }
                                             )
        );

        Assert.That(Factory.Upstream.Requests.Count, Is.EqualTo(1));
    }

    [Test]
    public void LegacyOmittedZeroRemainsAbsentWhileExplicitProhibitedKeepsItsOriginalWireTag()
    {
        var legacy = ContestDescription.Parser.ParseFrom(Array.Empty<byte>());
        var explicitProhibited = new ContestDescription { UpsolvingAllowance = UpsolvingAllowance.Prohibited };
        var parsed = ContestDescription.Parser.ParseFrom(explicitProhibited.ToByteArray());

        Assert.Multiple(() =>
            {
                Assert.That(legacy.UpsolvingAllowance, Is.EqualTo(UpsolvingAllowance.Prohibited));
                Assert.That(legacy.HasUpsolvingAllowance, Is.False);
                Assert.That(AllowanceOnWire(legacy), Is.Null);

                Assert.That(
                    explicitProhibited.ToByteArray(),
                    Is.EqualTo(
                        new byte[]
                        {
                            48,
                            0
                        }
                    )
                );

                Assert.That(parsed.HasUpsolvingAllowance, Is.True);
                Assert.That(parsed.UpsolvingAllowance, Is.EqualTo(UpsolvingAllowance.Prohibited));
            }
        );
    }

    [Test]
    public void UnknownWireEnumRemainsUnknownRatherThanBecomingProhibited()
    {
        var description = ContestDescription.Parser.ParseFrom(
            new byte[]
            {
                48,
                99
            }
        );

        Assert.Multiple(() =>
            {
                Assert.That(description.HasUpsolvingAllowance, Is.True);
                Assert.That((int)description.UpsolvingAllowance, Is.EqualTo(99));
                Assert.That(Enum.IsDefined(description.UpsolvingAllowance), Is.False);
            }
        );
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task CancellationReachesTheSelectedOutgoingRequest(bool ownerRequest)
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancelled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        if (ownerRequest)
            Factory.Upstream.EnqueueJson(Description(",\"upsolvingAllowance\":\"PROHIBITED\""));

        Factory.Upstream.Enqueue(async (_, token) =>
            {
                using var registration = token.Register(() => cancelled.TrySetResult());
                entered.TrySetResult();
                await Task.Delay(Timeout.InfiniteTimeSpan, token);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        );

        using var cancellation = new CancellationTokenSource();

        using var call = Api.Unary(
            ContestServiceName,
            "GetContest",
            new GetContestRequest
            {
                ContestId = 401,
                DescriptionOnly = !ownerRequest
            },
            GetContestResponse.Parser,
            cancellation.Token
        );

        await entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        cancellation.Cancel();
        Assert.ThrowsAsync<RpcException>(async () => await call.ResponseAsync);
        await cancelled.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.That(Factory.Upstream.Requests.Count, Is.EqualTo(ownerRequest ? 2 : 1));
    }

    private async Task<GetContestResponse> Read(GetContestRequest request) =>
        await Api.Unary(ContestServiceName, "GetContest", request, GetContestResponse.Parser).ResponseAsync;

    private static int? AllowanceOnWire(ContestDescription description)
    {
        using var input = new CodedInputStream(description.ToByteArray());
        uint tag;

        while ((tag = input.ReadTag()) != 0)
        {
            // The previous generated enum reader consumes this same tag and varint.
            if (tag == 48) return input.ReadEnum();

            input.SkipLastField();
        }

        return null;
    }

    private static string Description(string allowanceField) => $$"""
                                                                  {"name":"Synthetic descriptor","startTime":"2026-11-01T09:00:00Z","duration":7200,"type":"USUAL"{{allowanceField}}}
                                                                  """;
}
