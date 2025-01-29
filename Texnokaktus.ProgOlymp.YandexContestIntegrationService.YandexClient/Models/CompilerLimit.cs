using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record CompilerLimit
{
    [JsonPropertyName("compilerName")]
    public string CompilerName { get; set; }

    [JsonPropertyName("idlenessLimit")]
    public long IdlenessLimit { get; set; }

    [JsonPropertyName("memoryLimit")]
    public long MemoryLimit { get; set; }

    [JsonPropertyName("outputLimit")]
    public long OutputLimit { get; set; }

    [JsonPropertyName("timeLimit")]
    public long TimeLimit { get; set; }
}
