using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record ProblemResult
{
    [JsonPropertyName("score")]
    public string Score { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("submissionCount")]
    public string SubmissionCount { get; set; }

    [JsonPropertyName("submitDelay")]
    public long SubmitDelay { get; set; }
}
