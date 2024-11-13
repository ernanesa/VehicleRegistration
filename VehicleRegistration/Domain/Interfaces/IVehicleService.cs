using VehicleRegistration.Domain.Entities;

namespace VehicleRegistration.Domain.Interfaces;

public interface IVehicleService
{
    List<Vehicle> GetAll(int page, int pageSize, string name = null, string brand = null, int? year = null);
    Vehicle GetById(int id);
    Vehicle Create(Vehicle vehicle);
    Vehicle Update(Vehicle vehicle);
    void Delete(int id);
}