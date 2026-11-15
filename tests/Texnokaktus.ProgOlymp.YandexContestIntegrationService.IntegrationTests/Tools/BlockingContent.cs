using System.Net;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests.Tools;

public sealed class BlockingContent : HttpContent
{
    public TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource Cancelled { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public bool IsDisposed { get; private set; }

    public BlockingContent() => Headers.ContentType = new("application/json");

    protected override async Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken)
    {
        // HttpContent's default stream factory buffers through the tokenless overload.
        await SerializeToStreamAsync(Stream.Null, null, cancellationToken);

        return Stream.Null;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
        SerializeToStreamAsync(stream, context, CancellationToken.None);

    protected override async Task SerializeToStreamAsync(
        Stream stream,
        TransportContext? context,
        CancellationToken cancellationToken
    )
    {
        Entered.TrySetResult();

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Cancelled.TrySetResult();

            throw;
        }
    }

    protected override bool TryComputeLength(out long length)
    {
        length = 0;

        return false;
    }

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        base.Dispose(disposing);
    }
}
