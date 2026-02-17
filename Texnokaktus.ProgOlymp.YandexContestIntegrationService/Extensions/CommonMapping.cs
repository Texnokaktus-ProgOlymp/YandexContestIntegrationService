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
            StartTime = participantInfo.StartTime.ToTimestamp(),
            Uid = participantInfo.Uid
        };

        /// <summary>
        /// Converts a nullable duration in milliseconds into a Protobuf Duration object.
        /// If the input is null, returns null.
        /// </summary>
        /// <param name="durationMilliseconds">The nullable duration in milliseconds to convert.</param>
        /// <returns>A Protobuf Duration object representing the input duration, or null if the input is null.</returns>
        public static Duration? ToDuration(this long? durationMilliseconds) =>
            durationMilliseconds.HasValue
                ? TimeSpan.FromMilliseconds(durationMilliseconds.Value).ToDuration()
                : null;

        /// <summary>
        /// Converts a date and time string to a Protobuf Timestamp object.
        /// If the input string is not in a valid date and time format, returns null.
        /// </summary>
        /// <param name="dateTimeString">The date and time string to convert.</param>
        /// <returns>A Protobuf Timestamp object representing the parsed date and time, or null if parsing fails.</returns>
        public static Timestamp? ToTimestamp(this string? dateTimeString) =>
            DateTimeOffset.TryParse(dateTimeString, out var result)
                ? result.ToTimestamp()
                : null;
}
