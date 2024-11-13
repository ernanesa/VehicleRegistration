namespace VehicleRegistration.Domain.DTOs;

public record VehicleDTO
{
    public string Name { get; set; }
    public string Brand { get; set; }
    public int Year { get; set; }
}