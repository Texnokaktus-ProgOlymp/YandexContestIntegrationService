using Microsoft.AspNetCore.WebUtilities;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests;

[TestFixture]
public sealed class StandingsTests : SetupBase
{
    [TestCase("", false, 0L)]
    [TestCase(",\"totalEntitiesCount\":null", false, 0L)]
    [TestCase(",\"totalEntitiesCount\":0", true, 0L)]
    [TestCase(",\"totalEntitiesCount\":23", true, 23L)]
    public async Task ActualOptionalTotalAndRequestedPageArePreserved(string totalField, bool present, long expected)
    {
        Factory.Upstream.EnqueueJson($$"""{"rows":[],"titles":[]{{totalField}}}""");
        var response = await Read();
        var query = QueryHelpers.ParseQuery(Factory.Upstream.Requests.Single().Uri.Query);

        Assert.Multiple(() =>
            {
                Assert.That(response.Result.HasTotalEntitiesCount, Is.EqualTo(present));

                if (present)
                    Assert.That(response.Result.TotalEntitiesCount, Is.EqualTo(expected));

                Assert.That(response.RequestedPage.PageIndex, Is.EqualTo(3));
                Assert.That(response.RequestedPage.PageSize, Is.EqualTo(2));
                Assert.That(query["page"].ToString(), Is.EqualTo("3"));
                Assert.That(query["pageSize"].ToString(), Is.EqualTo("2"));
                Assert.That(query["forJudge"].ToString(), Is.EqualTo("true"));
            }
        );
    }

    [TestCase(null, false)]
    [TestCase("", true)]
    [TestCase("0", true)]
    [TestCase("1.005", true)]
    [TestCase("1,005", true)]
    public async Task RowAndProblemScoresRetainExactOriginalText(string? text, bool present)
    {
        var score = System.Text.Json.JsonSerializer.Serialize(text);

        Factory.Upstream.EnqueueJson(
            $$"""{"rows":[{"score":{{score}},"problemResults":[{"score":{{score}}}]}],"titles":[]}"""
        );

        var response = await Read();
        var row = response.Result.Rows.Single();
        var problem = row.ProblemResults.Single();

        Assert.Multiple(() =>
            {
                Assert.That(row.HasScoreText, Is.EqualTo(present));
                Assert.That(problem.HasScoreText, Is.EqualTo(present));

                if (present)
                {
                    Assert.That(row.ScoreText, Is.EqualTo(text));
                    Assert.That(problem.ScoreText, Is.EqualTo(text));
                }
            }
        );
    }

    private async Task<GetStandingsResponse> Read() => await Api.Unary(
                                                                     ContestServiceName,
                                                                     "GetStandings",
                                                                     new GetStandingsRequest
                                                                     {
                                                                         ContestId = 401,
                                                                         PageIndex = 3,
                                                                         PageSize = 2
                                                                     },
                                                                     GetStandingsResponse.Parser
                                                                 )
                                                                .ResponseAsync;
}
