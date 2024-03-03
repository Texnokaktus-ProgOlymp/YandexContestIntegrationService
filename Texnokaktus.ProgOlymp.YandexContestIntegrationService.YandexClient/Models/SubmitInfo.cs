using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record SubmitInfo
{
    [JsonPropertyName("participantId")]
    public long ParticipantId { get; set; }

    [JsonPropertyName("participantName")]
    public string ParticipantName { get; set; }

    [JsonPropertyName("problemTitle")]
    public string ProblemTitle { get; set; }

    [JsonPropertyName("submitTime")]
    public long SubmitTime { get; set; }
}
