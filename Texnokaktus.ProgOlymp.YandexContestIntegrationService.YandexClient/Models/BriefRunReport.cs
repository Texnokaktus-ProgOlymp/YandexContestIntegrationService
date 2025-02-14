namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record BriefRunReport
{
    public string Compiler { get; set; }
    public long MaxTimeUsage { get; set; }
    public long MaxMemoryUsage { get; set; }
    public string ProblemAlias { get; set; }
    public string ProblemId { get; set; }
    public long RunId { get; set; }
    public double? Score { get; set; }
    public DateTimeOffset SubmissionTime { get; set; }
    public int TestNumber { get; set; }
    public long TimeFromStart { get; set; }
    public string Verdict { get; set; }
}
