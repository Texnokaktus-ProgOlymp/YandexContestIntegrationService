using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Controllers;

public class RegistrationsController(IContestUserRepository repository) : Controller
{
    public async Task<IActionResult> Index()
    {
        var contestUsers = await repository.GetAllAsync();
        var registrations = contestUsers.Select(user => new ContestRegistration
        {
            Id = user.Id,
            ContestStageId = user.ContestStageId,
            YandexContestId = user.ContestStage.YandexContestId,
            YandexIdLogin = user.YandexIdLogin,
            ContestUserId = user.ContestUserId
        });
        return View(registrations);
    }
}
