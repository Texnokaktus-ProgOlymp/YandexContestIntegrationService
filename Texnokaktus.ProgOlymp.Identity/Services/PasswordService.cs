using System.Security.Cryptography;
using System.Text;
using Texnokaktus.ProgOlymp.Identity.Models;
using Texnokaktus.ProgOlymp.Identity.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Identity.Services;

/// <inheritdoc/>
internal class PasswordService : IPasswordService
{
    /// <inheritdoc/>
    public bool VerifyPassword(string password, PasswordData verifyData)
    {
        var passwordVerifyBytes = Convert.FromHexString(verifyData.PasswordHex);
        var passwordHash = GetPasswordHash(password, verifyData.SaltHex);
        return passwordHash.SequenceEqual(passwordVerifyBytes);
    }

    /// <summary>
    /// Calculates a SHA-256 hash for the input password.
    /// </summary>
    /// <param name="password">A password raw string.</param>
    /// <param name="saltHex">A salt hexadecimal string.</param>
    /// <returns>A byte array represents the hashed salt and password.</returns>
    private static IEnumerable<byte> GetPasswordHash(string password, string saltHex)
    {
        var passwordBytes = Encoding.Unicode.GetBytes(password);
        var saltBytes = Convert.FromHexString(saltHex);
        var data = saltBytes.Concat(passwordBytes).ToArray();
        return SHA256.HashData(data);
    }
}
