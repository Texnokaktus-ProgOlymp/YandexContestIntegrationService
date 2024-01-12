namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;

public class ContestStageApplication
{
    public int Id { get; init; }
    public int TransactionId { get; init; }
    public string YandexIdLogin { get; init; }
    public int ContestStageId { get; init; }
    public ApplicationState State { get; init; }
    public DateTime CreatedUtc { get; init; }
}
