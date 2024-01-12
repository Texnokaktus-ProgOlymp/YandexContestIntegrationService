namespace Texnokaktus.ProgOlymp.Identity.Exceptions;

/// <summary>
/// Represents an error that the user has entered incorrect credentials.
/// </summary>
public class IncorrectCredentialsException : AuthenticationException
{
    private const string TemplateMessage = "incorrect username or password";

    /// <summary>
    /// Initializes a new instance of <see cref="IncorrectCredentialsException"/>.
    /// </summary>
    internal IncorrectCredentialsException() : base(TemplateMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IncorrectCredentialsException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    internal IncorrectCredentialsException(string? message) : base($"{TemplateMessage}: {message}")
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IncorrectCredentialsException"/> with a specified error message and a
    /// reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    internal IncorrectCredentialsException(string? message,
                                           Exception? innerException) : base($"{TemplateMessage}: {message}",
                                                                             innerException)
    {
    }
}