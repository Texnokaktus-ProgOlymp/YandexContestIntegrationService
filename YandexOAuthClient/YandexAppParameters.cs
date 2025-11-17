namespace YandexOAuthClient;

public record YandexAppParameters
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }

    public void Deconstruct(out string clientId, out string clientSecret)
    {
        clientId = ClientId;
        clientSecret = ClientSecret;
    }
}
