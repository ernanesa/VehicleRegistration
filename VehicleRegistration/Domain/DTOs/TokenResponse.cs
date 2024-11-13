namespace VehicleRegistration.Domain.DTOs;

public record TokenResponse
{
    public string Token { get; init; } = default!;
    public DateTime Expiration { get; init; }
    public string TokenType { get; init; } = "Bearer";
}