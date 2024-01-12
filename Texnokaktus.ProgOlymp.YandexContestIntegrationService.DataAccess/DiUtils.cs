using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Context;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Repositories.Abstractions;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess;

public static class DiUtils
{
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection,
                                                   Action<DbContextOptionsBuilder> optionsAction) =>
        serviceCollection.AddDbContext<AppDbContext>(optionsAction)
                         .AddScoped<IUnitOfWork, UnitOfWork>()
                         .AddScoped<IContestStageApplicationRepository, ContestStageApplicationRepository>()
                         .AddScoped<IContestStageRepository, ContestStageRepository>();
}
