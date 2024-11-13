using VehicleRegistration.Domain.Entities;
using VehicleRegistration.Domain.Interfaces;
using VehicleRegistration.Infrastructure.Data;

namespace VehicleRegistration.Domain.Services;

public class VehicleService(VehicleRegistrationContext context) : IVehicleService
{
    private readonly VehicleRegistrationContext _context = context;

    public List<Vehicle> GetAll(int page, int pageSize, string name = null, string brand = null, int? year = null)
    {
        var vehicles = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrEmpty(name)) vehicles = vehicles.Where(v => v.Name.Contains(name));
        if (!string.IsNullOrEmpty(brand)) vehicles = vehicles.Where(v => v.Brand.Contains(brand));
        if (year.HasValue) vehicles = vehicles.Where(v => v.Year == year);

        return vehicles.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    public Vehicle GetById(int id)
    {
        var vehicle = _context.Vehicles.FirstOrDefault(v => v.Id == id) ?? throw new Exception("Vehicle not found");
        return vehicle;
    }

    public Vehicle Create(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
        return vehicle;
    }

    public Vehicle Update(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
        return vehicle;
    }

    public void Delete(int id)
    {
        _context.Vehicles.Remove(GetById(id));
        _context.SaveChanges();
    }
}