namespace Texnokaktus.ProgOlymp.Identity.Config;

public sealed record AuthenticationConfig
{
    public StubAuthentication StubAuthentication { get; init; }
}
