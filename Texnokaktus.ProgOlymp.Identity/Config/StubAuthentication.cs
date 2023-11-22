namespace Texnokaktus.ProgOlymp.Identity.Config;

public sealed record StubAuthentication
{
    public bool IsEnabled { get; init; }
    public string Login { get; init; }
    public string PasswordHex { get; init; }
    public string Salt { get; init; }
    public string[] Roles { get; init; }
}
