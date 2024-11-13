using System.ComponentModel.DataAnnotations;
using VehicleRegistration.Domain.Enums;
using VehicleRegistration.Domain.Validation;

namespace VehicleRegistration.Domain.DTOs;

public struct AdministratorDTO
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; init; } 

    [Required(ErrorMessage = "Password is required")]
    [PasswordValidation]
    public string Password { get; init; } 

    [Required(ErrorMessage = "Profile is required")]
    public ProfileEnum Profile { get; init; }
}