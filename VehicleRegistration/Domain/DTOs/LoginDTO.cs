using System.ComponentModel.DataAnnotations;

namespace VehicleRegistration.Domain.DTOs;

public record LoginDTO
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; init; } = default!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; init; } = default!;
}