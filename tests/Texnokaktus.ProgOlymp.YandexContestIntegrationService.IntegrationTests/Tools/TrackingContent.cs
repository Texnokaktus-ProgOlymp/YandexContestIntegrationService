using System.Text;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.IntegrationTests.Tools;

public sealed class TrackingContent(string json) : StringContent(json, Encoding.UTF8, "application/json")
{
    public bool IsDisposed { get; private set; }

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        base.Dispose(disposing);
    }
}
