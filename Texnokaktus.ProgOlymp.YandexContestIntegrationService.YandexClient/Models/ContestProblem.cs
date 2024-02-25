using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record ContestProblem
{
    [JsonPropertyName("alias")]
    public string Alias { get; set; }

    [JsonPropertyName("compilers")]
    public string[] Compilers { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("limits")]
    public CompilerLimit[] Limits { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("problemType")]
    public string ProblemType { get; set; }

    /*
    [JsonPropertyName("statements")]
    public Statement[] Statements { get; set; }
    */

    [JsonPropertyName("testCount")]
    public int TestCount { get; set; }
}
