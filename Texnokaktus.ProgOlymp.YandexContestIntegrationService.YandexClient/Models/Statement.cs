using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record Statement
{
    [JsonPropertyName("locale")] public string Locale { get; set; }
    [JsonPropertyName("path")] public string Path { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
}
