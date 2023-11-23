using System.Security.Claims;
using Microsoft.Extensions.Options;
using Texnokaktus.ProgOlymp.Identity.Config;
using Texnokaktus.ProgOlymp.Identity.Exceptions;
using Texnokaktus.ProgOlymp.Identity.Models;
using Texnokaktus.ProgOlymp.Identity.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Identity.Services;

/// <summary>
/// Represents a service that authenticates the user by username and password specified in the configuration.
/// <remarks>This service should be used only for development or testing purposes.
/// Do not use it in production.</remarks>
/// </summary>
/// <param name="configuration">An authentication configuration.</param>
/// <param name="passwordService">A password service.</param>
internal class StubIdentityService(IOptions<AuthenticationConfig> configuration, IPasswordService passwordService)
    : IIdentityService
{
    private const string AuthenticationScheme = "stubAccess";

    private readonly AuthenticationConfig _configuration = configuration.Value;


    /// <inheritdoc/>
    public ClaimsIdentity GetUserIdentity(string username, string password)
    {
        if (username != _configuration.StubAuthentication.Login)
            throw new IncorrectCredentialsException();

        var passwordData = new PasswordData(_configuration.StubAuthentication.PasswordHex,
                                            _configuration.StubAuthentication.Salt);

        if (!passwordService.VerifyPassword(password, passwordData))
            throw new IncorrectCredentialsException();

        var identity = new ClaimsIdentity(AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
        identity.AddClaim(new(ClaimTypes.NameIdentifier, username + Guid.NewGuid().ToString("N")));
        identity.AddClaim(new(ClaimTypes.Name, username, ClaimValueTypes.String));

        if (_configuration.StubAuthentication.Roles is null) return identity;

        foreach (var role in _configuration.StubAuthentication.Roles)
            identity.AddClaim(new(ClaimTypes.Role, role));

        return identity;
    }
}
