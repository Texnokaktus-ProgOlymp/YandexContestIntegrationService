namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Models;

public record LoginModel
{
    public string Username { get; init; }
    public string Password { get; init; }
    public string? RedirectUrl { get; set; }

    public void Deconstruct(out string username, out string password, out string? redirectUrl)
    {
        username = Username;
        password = Password;
        redirectUrl = RedirectUrl;
    }
}
