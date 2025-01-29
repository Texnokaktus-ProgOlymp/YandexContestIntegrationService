using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record ContestStatistics
{
    [JsonPropertyName("lastSubmit")]
    public SubmitInfo LastSubmit { get; set; }

    [JsonPropertyName("lastSuccess")]
    public SubmitInfo LastSuccess { get; set; }
}
