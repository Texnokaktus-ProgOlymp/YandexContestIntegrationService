using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record ParticipantStatus
{
    [JsonPropertyName("participantName")]
    public string Name { get; set; }

    [JsonPropertyName("participantStartTime")]
    public DateTimeOffset? StartTime { get; set; }

    [JsonPropertyName("participantFinishTime")]
    public DateTimeOffset? FinishTime { get; set; }

    [JsonPropertyName("participantLeftTimeMillis")]
    public int LeftTimeMilliseconds { get; set; }

    [JsonPropertyName("contestState")]
    public ParticipationState State { get; set; }
}
