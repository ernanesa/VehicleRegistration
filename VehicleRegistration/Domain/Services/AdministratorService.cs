using VehicleRegistration.Domain.DTOs;
using VehicleRegistration.Domain.Entities;
using VehicleRegistration.Domain.Interfaces;
using VehicleRegistration.Infrastructure.Data;

namespace VehicleRegistration.Domain.Services;

public class AdministratorService(VehicleRegistrationContext context) : IAdministratorService
{
    private readonly VehicleRegistrationContext _context = context;

    public Administrator Login(LoginDTO loginDto)
    {
        return _context.Administrators.FirstOrDefault(a => a.Email == loginDto.Email && a.Password == loginDto.Password);
    }

    public void Register(AdministratorDTO administrator)
    {
        _context.Administrators.Add(new Administrator { Email = administrator.Email, Password = administrator.Password, Profile = administrator.Profile.ToString() });
        _context.SaveChanges();
    }
}