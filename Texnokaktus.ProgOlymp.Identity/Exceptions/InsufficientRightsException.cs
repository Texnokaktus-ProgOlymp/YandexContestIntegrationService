namespace Texnokaktus.ProgOlymp.Identity.Exceptions;

/// <summary>
/// Represents an error that the user has no rights to access the application.
/// </summary>
public class InsufficientRightsException : AuthenticationException
{
    private const string TemplateMessage = "the user has insufficient access rights";

    /// <summary>
    /// Initializes a new instance of <see cref="InsufficientRightsException"/>.
    /// </summary>
    internal InsufficientRightsException() : base(TemplateMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="InsufficientRightsException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    internal InsufficientRightsException(string? message) : base($"{TemplateMessage}: {message}")
    {
    }
}
