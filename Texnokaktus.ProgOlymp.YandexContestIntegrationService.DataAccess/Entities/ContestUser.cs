namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

public class ContestUser
{
    public int Id { get; set; }
    public long ContestStageId { get; set; }
    public string YandexIdLogin { get; set; }
    public long ContestUserId { get; set; }
}
