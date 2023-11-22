using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.Identity.Config;
using Texnokaktus.ProgOlymp.Identity.Services;
using Texnokaktus.ProgOlymp.Identity.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Identity;

/// <summary>
/// Contains extension methods for adding identity services to the DI container.
/// </summary>
public static class IdentityDiExtensions
{
    /// <summary>
    /// Registers identity services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The app configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPasswordService, PasswordService>()
                .AddOptions<AuthenticationConfig>()
                .BindConfiguration("Authentication");
        var isStubEnabled = bool.Parse(configuration["Authentication:StubAuthentication:IsEnabled"]!);
        if (isStubEnabled) services.AddScoped<IIdentityService, StubIdentityService>();
        return services;
    }
}
