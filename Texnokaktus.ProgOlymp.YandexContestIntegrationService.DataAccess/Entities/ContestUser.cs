namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

public class ContestUser
{
    public required long ContestStageId { get; init; }
    public required string YandexIdLogin { get; init; }
    public required long ContestUserId { get; init; }
}
