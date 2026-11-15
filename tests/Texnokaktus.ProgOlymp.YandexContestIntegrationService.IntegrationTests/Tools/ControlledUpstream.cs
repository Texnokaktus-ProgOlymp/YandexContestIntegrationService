using System.Collections.Concurrent;
using System.Net;
using System.Text;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests.Tools;

public sealed class ControlledUpstream : HttpMessageHandler
{
    private readonly ConcurrentQueue<Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>> responses =
        new();

    public ConcurrentQueue<(HttpMethod Method, Uri Uri)> Requests { get; } = new();

    public void Enqueue(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> response) =>
        responses.Enqueue(response);

    public void EnqueueJson(string json) => Enqueue((_, _) => Task.FromResult(
                                                        new HttpResponseMessage(HttpStatusCode.OK)
                                                        {
                                                            Content = new StringContent(
                                                                json,
                                                                Encoding.UTF8,
                                                                "application/json"
                                                            )
                                                        }
                                                    )
    );

    public void EnqueueSource(string source, string? fileName = null) => Enqueue((_, _) =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(Encoding.UTF8.GetBytes(source))
            };

            response.Content.Headers.ContentType = new("application/octet-stream");

            if (fileName is not null)
                response.Content.Headers.ContentDisposition = new("attachment") { FileName = fileName };

            return Task.FromResult(response);
        }
    );

    public void Reset()
    {
        responses.Clear();
        Requests.Clear();
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        Requests.Enqueue((request.Method, request.RequestUri!));

        if (!responses.TryDequeue(out var response))
            throw new InvalidOperationException($"Unexpected upstream request: {request.Method} {request.RequestUri}");

        return response(request, cancellationToken);
    }
}
