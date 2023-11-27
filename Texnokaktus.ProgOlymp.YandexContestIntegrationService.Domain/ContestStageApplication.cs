namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;

public class ContestStageApplication
{
    public int Id { get; init; }
    public int AccountId { get; init; }
    public int ContestStageId { get; init; }
    public ApplicationState State { get; init; }
    public DateTime CreatedUtc { get; init; }
}
