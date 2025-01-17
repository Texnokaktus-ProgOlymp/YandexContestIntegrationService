using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public enum ParticipationState
{
    [JsonStringEnumMemberName("NOT_STARTED")]
    NotStarted,

    [JsonStringEnumMemberName("IN_PROGRESS")]
    InProgress,

    [JsonStringEnumMemberName("FINISHED")]
    Finished
}
