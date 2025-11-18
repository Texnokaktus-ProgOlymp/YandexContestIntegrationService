namespace YandexOAuthClient;

public record TokenSet
{
    public required string TokenType { get; set; }
    public required string AccessToken { get; set; }
    public required string? RefreshToken { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
    public required string Scope { get; set; }
}
