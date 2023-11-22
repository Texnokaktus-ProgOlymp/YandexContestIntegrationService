using Texnokaktus.ProgOlymp.Identity.Models;

namespace Texnokaktus.ProgOlymp.Identity.Services.Abstractions;

/// <summary>
/// Represents a service that verifies passwords.
/// </summary>
internal interface IPasswordService
{
    /// <summary>
    /// Verifies a password.
    /// </summary>
    /// <param name="password">The password entered by the user.</param>
    /// <param name="verifyData">The correct password hashed data.</param>
    /// <returns>True if the password is correct. Otherwise, false.</returns>
    bool VerifyPassword(string password, PasswordData verifyData);
}
