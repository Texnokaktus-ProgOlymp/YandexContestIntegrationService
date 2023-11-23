namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

public class ContestStageApplication
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int ContestStageId { get; set; }
    public ApplicationState State { get; set; }
    public DateTime CreatedUtc { get; set; }
}
