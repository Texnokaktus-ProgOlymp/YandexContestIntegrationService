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

    [TestCase("", false, "")]
    [TestCase(",\"id\":null", false, "")]
    [TestCase(",\"id\":\"\"", true, "")]
    [TestCase(",\"id\":\"0007-a/b\"", true, "0007-a/b")]
    public async Task TitleIdRetainsPresenceAndExactOpaqueValue(string idField, bool present, string expected)
    {
        Factory.Upstream.EnqueueJson(
            $$"""{"rows":[],"titles":[{"name":"alias","title":"Visible title"{{idField}}}]}"""
        );

        var response = await Read();
        var title = response.Result.Titles.Single();
        Assert.That(title.HasId, Is.EqualTo(present));
        Assert.That(title.Id, Is.EqualTo(expected));
        Assert.That(title.Name, Is.EqualTo("alias"));
        Assert.That(title.Title, Is.EqualTo("Visible title"));
    }

    [Test]
    public async Task TwoTitleIdentitiesStayAlignedWithPositionalProblemScores()
    {
        Factory.Upstream.EnqueueJson(
            """
            {"titles":[{"id":"0007-a/b","name":"same alias","title":"Alpha"},
                       {"id":"second","name":"same alias","title":"Beta"}],
             "rows":[{"problemResults":[{"score":"1.005"},{"score":"0"}]}]}
            """
        );

        var response = await Read();
        var titles = response.Result.Titles;
        var results = response.Result.Rows.Single().ProblemResults;
        Assert.That(titles.Count, Is.EqualTo(2));
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(titles.All(title => title.HasId), Is.True);
        Assert.That(results.All(result => result.HasScoreText), Is.True);

        Assert.That(
            titles.Zip(results, (title, result) => (title.Id, title.Title, result.ScoreText)),
            Is.EqualTo(
                new[]
                {
                    ("0007-a/b", "Alpha", "1.005"),
                    ("second", "Beta", "0")
                }
            )
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
