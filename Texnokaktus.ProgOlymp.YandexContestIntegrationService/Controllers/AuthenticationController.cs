using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.Identity.Exceptions;
using Texnokaktus.ProgOlymp.Identity.Services.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Controllers;

public class AuthenticationController(ILogger<AuthenticationController> logger, IIdentityService identityService) : Controller
{
    private const string IncorrectCredentials = "Incorrect username or password.";
    private const string InsufficientRights = "You don't have access rights to this site.";
    private const string TechnicalError = "A technical error occurred. Please try again later.";

    [HttpGet]
    public IActionResult Login(string redirectUrl)
    {
        return View(new LoginModel { RedirectUrl = redirectUrl });
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginModel loginModel)
    {
        logger.LogInformation("POST Login {@Model}", loginModel);

        ClaimsIdentity userIdentity;
        try
        {
            userIdentity = identityService.GetUserIdentity(loginModel.Username, loginModel.Password);
        }
        catch (IncorrectCredentialsException)
        {
            ViewData["ErrorMessage"] = IncorrectCredentials;
            return View(loginModel);
        }
        catch (InsufficientRightsException)
        {
            ViewData["ErrorMessage"] = InsufficientRights;
            return View(loginModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured during authentication");
            ViewData["ErrorMessage"] = TechnicalError;
            return View(loginModel);
        }

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
