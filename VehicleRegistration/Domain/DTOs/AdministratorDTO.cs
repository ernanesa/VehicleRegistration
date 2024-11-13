using VehicleRegistration.Domain.Enums;

namespace VehicleRegistration.Domain.DTOs;

public struct AdministratorDTO
{
    public string  Email { get; set; }
    public string Password { get; set; }
    public ProfileEnum Profile { get; set; }
}