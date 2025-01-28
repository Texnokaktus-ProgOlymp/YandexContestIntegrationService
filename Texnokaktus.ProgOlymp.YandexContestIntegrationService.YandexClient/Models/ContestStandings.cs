using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record ContestStandings
{
    [JsonPropertyName("rows")]
    public ContestStandingsRow[] Rows { get; set; }

    [JsonPropertyName("statistics")]
    public ContestStatistics Statistics { get; set; }

    [JsonPropertyName("titles")]
    public ContestStandingsTitle[] Titles { get; set; }
}
