namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;

public class ContestStageApplication
{
    public required int Id { get; init; }
    public required int TransactionId { get; init; }
    public required string YandexIdLogin { get; init; }
    public required int ContestStageId { get; init; }
    public required long? YandexContestId { get; init; }
    public required ApplicationState State { get; init; }
    public required DateTime CreatedUtc { get; init; }
}
