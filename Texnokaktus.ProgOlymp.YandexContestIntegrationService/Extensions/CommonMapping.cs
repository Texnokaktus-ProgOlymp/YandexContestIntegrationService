using Google.Protobuf.WellKnownTypes;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.YandexContest;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Extensions;

internal static class CommonMapping
{
    public static ParticipantInfo MapParticipantInfo(this YandexContestClient.Client.Models.ParticipantInfo participantInfo) =>
        new()
        {
            Id = participantInfo.Id ?? 0L,
            Login = participantInfo.Login,
            Name = participantInfo.Name,
            StartTime = DateTimeOffset.TryParse(participantInfo.StartTime, out var result)
                            ? result.ToTimestamp()
                            : null,
            Uid = participantInfo.Uid
        };
}
