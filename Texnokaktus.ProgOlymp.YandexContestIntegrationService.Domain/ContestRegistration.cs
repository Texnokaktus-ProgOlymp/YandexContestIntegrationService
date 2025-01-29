namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;

public class ContestRegistration
{
    public int Id { get; init; }
    public int ContestStageId { get; init; }
    public long? YandexContestId { get; init; }
    public string YandexIdLogin { get; init; }
    public long ContestUserId { get; init; }
}
