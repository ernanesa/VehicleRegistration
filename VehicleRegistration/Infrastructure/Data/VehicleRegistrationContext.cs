using Microsoft.EntityFrameworkCore;
using VehicleRegistration.Domain.Entities;

namespace VehicleRegistration.Infrastructure.Data
{
    public class VehicleRegistrationContext(DbContextOptions<VehicleRegistrationContext> options) : DbContext(options)
    {
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>().HasData(
                new Administrator { Id = 1, Email = "admin@teste.com", Password = "admin", Profile = "admin" }
            );
        }
    }
}