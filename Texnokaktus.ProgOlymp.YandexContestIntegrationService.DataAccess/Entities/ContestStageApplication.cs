namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

public class ContestStageApplication
{
    public int Id { get; init; }
    public int TransactionId { get; init; }
    public string YandexIdLogin { get; init; }
    public int ContestStageId { get; init; }
    public ContestStage ContestStage { get; init; }
    public ApplicationState State { get; set; }
    public DateTime CreatedUtc { get; init; }
}
