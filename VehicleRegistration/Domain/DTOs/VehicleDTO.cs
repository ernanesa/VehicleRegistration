using System.ComponentModel.DataAnnotations;

namespace VehicleRegistration.Domain.DTOs;

public record VehicleDTO
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters")]
    public string Name { get; init; } = default!;

    [Required(ErrorMessage = "Brand is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Brand must be between 2 and 100 characters")]
    public string Brand { get; init; } = default!;

    [Required(ErrorMessage = "Year is required")]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int Year { get; init; }
}