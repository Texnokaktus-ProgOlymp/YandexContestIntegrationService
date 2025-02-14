namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record ParticipantStats
{
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FirstSubmissionTime { get; set; }
    public BriefRunReport[] Runs { get; set; }
}
