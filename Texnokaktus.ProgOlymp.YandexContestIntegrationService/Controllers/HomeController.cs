using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Models;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction(nameof(RegistrationsController.Index), "Registrations");

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });

    [Route("[controller]/error/{id:int}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ErrorId(int id) => id switch
    {
        (int)HttpStatusCode.NotFound => View("NotFound"),
        _                            => View("Error",
                                             new ErrorViewModel
                                             {
                                                 RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                                             })
    };
}
