namespace Texnokaktus.ProgOlymp.Identity.Exceptions;

/// <summary>
/// Represents a base error during authentication.
/// </summary>
public class AuthenticationException : Exception
{
    private const string TemplateMessage = "An error occurred during authentication";

    /// <summary>
    /// Initializes a new instance of <see cref="AuthenticationException"/>.
    /// </summary>
    internal AuthenticationException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="AuthenticationException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    internal AuthenticationException(string? message) : base($"{TemplateMessage}: {message}")
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="AuthenticationException"/> with a specified error message and a
    /// reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    internal AuthenticationException(string? message, Exception? innerException) : base($"{TemplateMessage}: {message}", innerException)
    {
    }
}
