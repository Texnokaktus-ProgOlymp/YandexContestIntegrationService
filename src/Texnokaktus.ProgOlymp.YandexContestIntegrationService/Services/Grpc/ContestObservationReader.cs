using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Services.Grpc;

// The typed model and selected raw fields always come from the same response.
internal static class ContestObservationReader
{
    internal static async Task<(T Model, string Json)> ReadAsync<T>(
        NativeResponseHandler handler,
        ParsableFactory<T> factory,
        CancellationToken cancellationToken
    )
        where T : IParsable
    {
        using var response = handler.Value as HttpResponseMessage
                          ?? throw new RpcException(new(StatusCode.Unavailable, "Contest returned no HTTP response."));

        if (!response.IsSuccessStatusCode)
        {
            var status = response.StatusCode switch
            {
                HttpStatusCode.Unauthorized           => StatusCode.Unauthenticated,
                HttpStatusCode.Forbidden              => StatusCode.PermissionDenied,
                HttpStatusCode.NotFound               => StatusCode.NotFound,
                HttpStatusCode.TooManyRequests        => StatusCode.ResourceExhausted,
                >= HttpStatusCode.InternalServerError => StatusCode.Unavailable,
                _                                     => StatusCode.FailedPrecondition
            };

            throw new RpcException(new(status, "Contest rejected the request."));
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            var model = await KiotaJsonSerializer.DeserializeAsync(json, factory, cancellationToken)
                     ?? throw new JsonException("Missing response object.");

            return (model, json);
        }
        catch (Exception exception) when (exception is JsonException or FormatException or InvalidOperationException)
        {
            throw new RpcException(new(StatusCode.DataLoss, "Contest returned an invalid response."));
        }
    }

    internal static long? Int64(JsonElement source, string propertyName)
    {
        if (!source.TryGetProperty(propertyName, out var value) || value.ValueKind == JsonValueKind.Null) return null;
        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out var result)) return result;

        throw new RpcException(new(StatusCode.DataLoss, "Contest returned an invalid identity."));
    }

    internal static string? Text(JsonElement source, string propertyName) =>
        source.TryGetProperty(propertyName, out var value)
            ? value.ValueKind switch
            {
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.GetRawText(),
                JsonValueKind.Null => null,
                _ => throw new RpcException(new(StatusCode.DataLoss, "Contest returned an invalid field."))
            }
            : null;

    internal static Timestamp? Timestamp(string? value) =>
        DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed.ToTimestamp()
            : null;

    internal static Duration? Milliseconds(long? value) => value is { } milliseconds
                                                               ? new Duration
                                                               {
                                                                   Seconds = milliseconds / 1000,
                                                                   Nanos = (int)(milliseconds % 1000) * 1_000_000
                                                               }
                                                               : null;

    internal static string TechnicalJson(JsonElement source, params string[] properties)
    {
        using var buffer = new MemoryStream();

        using (var writer = new Utf8JsonWriter(buffer))
        {
            writer.WriteStartObject();

            foreach (var name in properties)
            {
                if (!source.TryGetProperty(name, out var value)) continue;

                writer.WritePropertyName(name);

                if (name == "participantInfo" && value.ValueKind == JsonValueKind.Object)
                {
                    writer.WriteStartObject();

                    if (value.TryGetProperty("id", out var id))
                    {
                        writer.WritePropertyName("id");
                        writer.WriteRawValue(id.GetRawText());
                    }

                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteRawValue(value.GetRawText());
                }
            }

            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }
}
