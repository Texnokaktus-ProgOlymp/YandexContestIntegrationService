using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record ContestProblems
{
    [JsonPropertyName("problems")]
    public ContestProblem[] Problems { get; set; }
}
