﻿using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services;
using Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic;

public static class DiExtensions
{
    public static IServiceCollection AddLogicLayerServices(this IServiceCollection services) =>
        services.AddScoped<IContestStageService, ContestStageService>()
                .AddScoped<IRegistrationService, RegistrationService>()
                .AddScoped<IParticipantService, ParticipantService>();
}
