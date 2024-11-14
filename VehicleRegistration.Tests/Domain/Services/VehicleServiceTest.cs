using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using VehicleRegistration.Domain.Entities;
using VehicleRegistration.Domain.Services;
using VehicleRegistration.Infrastructure.Data;

namespace VehicleRegistration.Tests.Domain.Services;

public class VehicleServiceTests
{
    private readonly DbContextOptions<VehicleRegistrationContext> _options;
    private readonly List<Vehicle> _testVehicles;

    public VehicleServiceTests()
    {
        // Setup in-memory database
        _options = new DbContextOptionsBuilder<VehicleRegistrationContext>()
            .UseInMemoryDatabase(databaseName: "TestVehicleDb")
            .Options;

        // Setup test data
        _testVehicles = new List<Vehicle>
        {
            new() { Id = 1, Name = "Civic", Brand = "Honda", Year = 2020 },
            new() { Id = 2, Name = "Corolla", Brand = "Toyota", Year = 2021 },
            new() { Id = 3, Name = "Model 3", Brand = "Tesla", Year = 2022 }
        };

        // Initialize database with test data
        using var context = new VehicleRegistrationContext(_options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        context.Vehicles.AddRange(_testVehicles);
        context.SaveChanges();
    }

    [Fact]
    public void GetAll_WithoutFilters_ReturnsAllVehicles()
    {
        // Arrange
        using var context = new VehicleRegistrationContext(_options);
        var service = new VehicleService(context);

        // Act
        var result = service.GetAll(1, 10);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(_testVehicles.Count, result.Count);
        Assert.Equal(_testVehicles[0].Name, result[0].Name);
    }

    [Theory]
    [InlineData("Civic", "Honda", 2020, 1)]
    [InlineData("Corolla", null, null, 1)]
    [InlineData(null, "Tesla", null, 1)]
    public void GetAll_WithFilters_ReturnsFilteredVehicles(string name, string brand, int? year, int expectedCount)
    {
        // Arrange
        using var context = new VehicleRegistrationContext(_options);
        var service = new VehicleService(context);

        // Act
        var result = service.GetAll(1, 10, name, brand, year);

        // Assert
        Assert.Equal(expectedCount, result.Count);
        if (name != null)
            Assert.All(result, v => Assert.Contains(name, v.Name));
        if (brand != null)
            Assert.All(result, v => Assert.Contains(brand, v.Brand));
        if (year.HasValue)
            Assert.All(result, v => Assert.Equal(year.Value, v.Year));
    }

    [Fact]
    public void GetById_ExistingId_ReturnsVehicle()
    {
        // Arrange
        using var context = new VehicleRegistrationContext(_options);
        var service = new VehicleService(context);

        // Act
        var result = service.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Civic", result.Name);
    }

    [Fact]
    public void GetById_NonExistingId_ThrowsException()
    {
        // Arrange
        using var context = new VehicleRegistrationContext(_options);
        var service = new VehicleService(context);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.GetById(999));
        Assert.Equal("Vehicle not found", exception.Message);
    }

    [Fact]
    public void Create_ValidVehicle_ReturnsCreatedVehicle()
    {
        // Arrange
        using var context = new VehicleRegistrationContext(_options);
        var service = new VehicleService(context);
        var newVehicle = new Vehicle { Name = "X5", Brand = "BMW", Year = 2023 };

        // Act
        var result = service.Create(newVehicle);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newVehicle.Name, result.Name);
        Assert.Equal(newVehicle.Brand, result.Brand);
        Assert.Equal(newVehicle.Year, result.Year);

        // Verify it was actually saved to the database
        var savedVehicle = context.Vehicles.FirstOrDefault(v => v.Name == "X5");
        Assert.NotNull(savedVehicle);
    }

    [Fact]
    public void Update_ExistingVehicle_ReturnsUpdatedVehicle()
    {
        // Arrange
        using var context = new VehicleRegistrationContext(_options);
        var service = new VehicleService(context);
        var existingVehicle = context.Vehicles.First();
        var updatedVehicle = new Vehicle 
        { 
            Id = existingVehicle.Id,
            Name = "Civic Updated",
            Brand = "Honda",
            Year = 2021
        };

        // Act
        var result = service.Update(updatedVehicle);

        // Assert
        Assert.Equal(updatedVehicle.Name, result.Name);
        Assert.Equal(updatedVehicle.Brand, result.Brand);
        Assert.Equal(updatedVehicle.Year, result.Year);

        // Verify it was actually updated in the database
        var savedVehicle = context.Vehicles.Find(existingVehicle.Id);
        Assert.Equal("Civic Updated", savedVehicle.Name);
    }

    [Fact]
    public void Delete_ExistingId_RemovesVehicle()
    {
        // Arrange
        using var context = new VehicleRegistrationContext(_options);
        var service = new VehicleService(context);
        var vehicleToDelete = context.Vehicles.First();

        // Act
        service.Delete(vehicleToDelete.Id);

        // Assert
        var deletedVehicle = context.Vehicles.FirstOrDefault(v => v.Id == vehicleToDelete.Id);
        Assert.Null(deletedVehicle);
    }

    [Fact]
    public void Delete_NonExistingId_ThrowsException()
    {
        // Arrange
        using var context = new VehicleRegistrationContext(_options);
        var service = new VehicleService(context);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.Delete(999));
        Assert.Equal("Vehicle not found", exception.Message);
    }
}