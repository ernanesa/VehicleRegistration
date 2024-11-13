using VehicleRegistration.Domain.DTOs;
using VehicleRegistration.Domain.Entities;

namespace VehicleRegistration.Domain.Interfaces; 

public interface IAdministratorService
{
    Administrator Login(LoginDTO loginDto);
    void Register(AdministratorDTO administrator);
}