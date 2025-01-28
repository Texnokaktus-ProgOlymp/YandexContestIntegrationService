using System.Text.Json.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Models;

public record UpdateParticipantRequest([property: JsonPropertyName("displayedName")]string DisplayedName);
