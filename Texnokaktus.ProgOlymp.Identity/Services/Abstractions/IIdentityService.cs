using System.Security.Claims;
using Texnokaktus.ProgOlymp.Identity.Exceptions;

namespace Texnokaktus.ProgOlymp.Identity.Services.Abstractions;

public interface IIdentityService
{
    /// <summary>
    /// Gets user claims identity.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The user's password</param>
    /// <exception cref="IncorrectCredentialsException">Username or password is incorrect.</exception>
    /// <exception cref="InsufficientRightsException">The user is not a member of the required groups.</exception>
    /// <returns>Claims identity.</returns>
    ClaimsIdentity GetUserIdentity(string username, string password);
}
