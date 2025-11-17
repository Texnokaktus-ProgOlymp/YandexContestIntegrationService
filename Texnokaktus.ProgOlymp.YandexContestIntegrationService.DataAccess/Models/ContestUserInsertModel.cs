namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Models;

public record ContestUserInsertModel(int ParticipantId, long ContestStageId, string YandexIdLogin, long ContestUserId);
