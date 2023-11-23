using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Models;

public record ContestStageApplicationInsertModel(int AccountId, int ContestStageId, ApplicationState State, DateTime CreatedUtc);
