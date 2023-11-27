using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Controllers;

[Authorize(Roles = "yandex-contest-admin")]
public class ApplicationsController(IApplicationService applicationService) : Controller
{
    public async Task<IActionResult> IndexAsync()
    {
        var applications = await applicationService.GetApplicationsAsync();
        return View(applications);
    }

    [Route("[controller]/{id}")]
    public async Task<IActionResult> ItemAsync(int id)
    {
        var application = await applicationService.GetApplicationAsync(id);
        if (application is null) return NotFound();
        return View(application);
    }
}
