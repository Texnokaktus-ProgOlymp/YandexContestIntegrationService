using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public enum UpsolvingAllowance
{
    [JsonStringEnumMemberName("PROHIBITED")]
    Prohibited,

    [JsonStringEnumMemberName("ALLOWED_AFTER_PARTICIPATION_ENDS")]
    AllowedAfterParticipationEnds,

    [JsonStringEnumMemberName("ALLOWED_AFTER_CONTEST_ENDS")]
    AllowedAfterContestEnds
}
