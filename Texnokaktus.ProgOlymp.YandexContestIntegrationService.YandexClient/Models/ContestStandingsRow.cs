using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record ContestStandingsRow
{
    [JsonPropertyName("participantInfo")]
    public ParticipantInfo ParticipantInfo { get; set; }

    [JsonPropertyName("placeFrom")]
    public int[] PlaceFrom { get; set; }

    [JsonPropertyName("placeTo")]
    public int[] PlaceTo { get; set; }

    [JsonPropertyName("problemResults")]
    public ProblemResult[] ProblemResults { get; set; }

    [JsonPropertyName("score")]
    public string Score { get; set; }
}
