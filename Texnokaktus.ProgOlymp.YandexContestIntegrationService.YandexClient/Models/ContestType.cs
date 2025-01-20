using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public enum ContestType
{
    [JsonStringEnumMemberName("USUAL")]
    Usual,

    [JsonStringEnumMemberName("VIRTUAL")]
    Virtual
}
