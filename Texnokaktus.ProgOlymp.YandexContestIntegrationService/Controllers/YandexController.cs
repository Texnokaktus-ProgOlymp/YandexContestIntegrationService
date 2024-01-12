using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Models;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Controllers;

public class YandexController(LinkGenerator linkGenerator,
                              IYandexAuthenticationService yandexAuthenticationService,
                              ITokenService tokenService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var accessToken = await tokenService.GetAccessTokenAsync();
        return View(accessToken is not null);
    }

    public IActionResult Auth()
    {
        var authUrl = yandexAuthenticationService.GetYandexOAuthUrl(RedirectUri);
        return Redirect(authUrl);
    }

    public IActionResult Complete(string? code,
                                  string? error,
                                  [FromQuery(Name = "error_description")]
                                  string? errorDescription) =>
        (code, error) switch
        {
            (not null, null) => RedirectToAction(nameof(Code), new { code }),
            (null, not null) => RedirectToAction(nameof(Error), new { error, errorDescription }),
            _                => throw new("Invalid Yandex response.")
        };

    public async Task<IActionResult> Code(string code)
    {
        var response = await yandexAuthenticationService.GetAccessTokenAsync(code);
        await tokenService.RegisterTokenAsync(response);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Error(string error, string errorDescription) =>
        View("Error", new YandexAuthError(error, errorDescription));

    private string RedirectUri => linkGenerator.GetUriByAction(HttpContext, nameof(Complete))!;
}
