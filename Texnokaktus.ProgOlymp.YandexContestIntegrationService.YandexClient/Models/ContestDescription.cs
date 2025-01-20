using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record ContestDescription
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("startTime")]
    public DateTimeOffset StartTime { get; set; }

    [JsonPropertyName("duration")]
    public long DurationSeconds { get; set; }

    [JsonPropertyName("freezeTime")]
    public long? FreezeTimeSeconds { get; set; }

    [JsonPropertyName("type")]
    public ContestType Type { get; set; }

    [JsonPropertyName("upsolvingAllowance")]
    public UpsolvingAllowance UpsolvingAllowance { get; set; }
}
