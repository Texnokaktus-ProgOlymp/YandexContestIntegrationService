namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

public class ContestUser
{
    public int Id { get; set; }
    public ContestStage ContestStage { get; set; }
    public int ContestStageId { get; set; }
    public string YandexIdLogin { get; set; }
    public long ContestUserId { get; set; }
}
