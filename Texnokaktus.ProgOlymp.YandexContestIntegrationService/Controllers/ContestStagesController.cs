using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Controllers;

[Authorize(Roles = "yandex-contest-admin")]
public class ContestStagesController(IContestStageService contestStageService) : Controller
{
    public async Task<IActionResult> IndexAsync()
    {
        var applications = await contestStageService.GetContestStagesAsync();
        return View(applications);
    }

    [Route("[controller]/{id:int}")]
    public async Task<IActionResult> ItemAsync(int id)
    {
        var application = await contestStageService.GetContestStageAsync(id);
        if (application is null) return NotFound();
        return View(application);
    }

    [HttpPost]
    [Route("[controller]/{id:int}")]
    public async Task<IActionResult> UpdateItemAsync(int id, ContestStage contestStage)
    {
        if (contestStage.YandexContestId is { } yandexContestId)
            await contestStageService.SetYandexContestIdAsync(id, yandexContestId);
        return RedirectToAction("Item", "ContestStages", new { Id = id });
    }
}
