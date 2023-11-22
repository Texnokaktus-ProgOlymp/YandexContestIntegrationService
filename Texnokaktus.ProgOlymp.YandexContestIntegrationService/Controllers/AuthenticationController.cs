using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.Identity.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Controllers;

public class AuthenticationController(ILogger<AuthenticationController> logger, IIdentityService identityService) : Controller
{
    public IActionResult Index(string redirectUrl)
    {
        return View(new LoginModel { RedirectUrl = redirectUrl });
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginModel loginModel)
    {
        logger.LogInformation("POST Login {@Model}", loginModel);

        var userIdentity = identityService.GetUserIdentity(loginModel.Username, loginModel.Password);

        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(userIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        if (loginModel.RedirectUrl is not null)
            return Redirect(loginModel.RedirectUrl);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> LogoutAsync()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
