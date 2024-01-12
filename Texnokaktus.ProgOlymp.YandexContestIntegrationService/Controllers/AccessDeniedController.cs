using Microsoft.AspNetCore.Mvc;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Controllers;

public class AccessDeniedController(ILogger<AccessDeniedController> logger) : Controller
{
    public IActionResult Index()
    {
        logger.LogWarning("The user {UserName} tried to access the restricted resource", User.Identity?.Name);
        return View();
    }
}
