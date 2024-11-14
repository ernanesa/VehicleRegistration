using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VehicleRegistration.Domain.Entities;
using VehicleRegistration.Domain.Interfaces;
using VehicleRegistration.Endpoints;

namespace VehicleRegistration.Tests.Endpoints
{
    public class VehicleEndpointsTests
    {
        private readonly Mock<IVehicleService> _vehicleServiceMock;
        private readonly WebApplication _app;
        private readonly IConfiguration _configuration;

        public VehicleEndpointsTests()
        {
            _vehicleServiceMock = new Mock<IVehicleService>();

            // Setup configuration to read from appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            // Setup WebApplication
            var appBuilder = WebApplication.CreateBuilder();

            // Add configuration to the web application
            appBuilder.Configuration.AddConfiguration(_configuration);

            // Configure services
            appBuilder.Services.AddSingleton(_vehicleServiceMock.Object);

            _app = appBuilder.Build();

            // Map endpoints
            VehicleEndpoints.MapVehicleEndpoints(_app);
        }

        [Fact]
        public void GetAll_ReturnsOkWithVehicles()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, Name = "Test Car 1", Brand = "Test Brand", Year = 2023 },
                new Vehicle { Id = 2, Name = "Test Car 2", Brand = "Test Brand", Year = 2024 }
            };

            _vehicleServiceMock
                .Setup(x => x.GetAll(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .Returns(vehicles);

            var vehicleService = _app.Services.GetRequiredService<IVehicleService>();

            // Act
            var result = vehicleService.GetAll(1, 10, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _vehicleServiceMock.Verify(x => x.GetAll(1, 10, null, null, null), Times.Once);
        }

        [Fact]
        public void GetById_WithValidId_ReturnsVehicle()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "Test Car",
                Brand = "Test Brand",
                Year = 2023
            };

            _vehicleServiceMock
                .Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(vehicle);

            var vehicleService = _app.Services.GetRequiredService<IVehicleService>();

            // Act
            var result = vehicleService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicle.Id, result.Id);
            Assert.Equal(vehicle.Name, result.Name);
            _vehicleServiceMock.Verify(x => x.GetById(1), Times.Once);
        }

        [Fact]
        public void Create_WithValidVehicle_ReturnsCreated()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Name = "New Car",
                Brand = "New Brand",
                Year = 2024
            };

            var createdVehicle = new Vehicle
            {
                Id = 1,
                Name = vehicle.Name,
                Brand = vehicle.Brand,
                Year = vehicle.Year
            };

            _vehicleServiceMock
                .Setup(x => x.Create(It.IsAny<Vehicle>()))
                .Returns(createdVehicle);

            var vehicleService = _app.Services.GetRequiredService<IVehicleService>();

            // Act
            var result = vehicleService.Create(vehicle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(vehicle.Name, result.Name);
            _vehicleServiceMock.Verify(x => x.Create(It.Is<Vehicle>(
                v => v.Name == vehicle.Name &&
                     v.Brand == vehicle.Brand &&
                     v.Year == vehicle.Year)),
                Times.Once);
        }

        [Fact]
        public void Update_WithValidVehicle_ReturnsUpdatedVehicle()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "Updated Car",
                Brand = "Updated Brand",
                Year = 2024
            };

            _vehicleServiceMock
                .Setup(x => x.Update(It.IsAny<Vehicle>()))
                .Returns(vehicle);

            var vehicleService = _app.Services.GetRequiredService<IVehicleService>();

            // Act
            var result = vehicleService.Update(vehicle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicle.Id, result.Id);
            Assert.Equal(vehicle.Name, result.Name);
            _vehicleServiceMock.Verify(x => x.Update(It.Is<Vehicle>(
                v => v.Id == vehicle.Id &&
                     v.Name == vehicle.Name &&
                     v.Brand == vehicle.Brand &&
                     v.Year == vehicle.Year)),
                Times.Once);
        }

        [Fact]
        public void Delete_WithValidId_CallsDeleteOnce()
        {
            // Arrange
            _vehicleServiceMock
                .Setup(x => x.Delete(It.IsAny<int>()))
                .Verifiable();

            var vehicleService = _app.Services.GetRequiredService<IVehicleService>();

            // Act
            vehicleService.Delete(1);

            // Assert
            _vehicleServiceMock.Verify(x => x.Delete(1), Times.Once);
        }

        [Fact]
        public void GetAll_WithFilters_ReturnsFilteredVehicles()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, Name = "Specific Car", Brand = "Specific Brand", Year = 2023 }
            };

            _vehicleServiceMock
                .Setup(x => x.GetAll(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.Is<string>(n => n == "Specific Car"),
                    It.Is<string>(b => b == "Specific Brand"),
                    It.Is<int?>(y => y == 2023)))
                .Returns(vehicles);

            var vehicleService = _app.Services.GetRequiredService<IVehicleService>();

            // Act
            var result = vehicleService.GetAll(1, 10, "Specific Car", "Specific Brand", 2023);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Specific Car", result.First().Name);
            _vehicleServiceMock.Verify(x => x.GetAll(1, 10, "Specific Car", "Specific Brand", 2023), Times.Once);
        }

        [Fact]
        public void GetById_WithInvalidId_ThrowsException()
        {
            // Arrange
            _vehicleServiceMock
                .Setup(x => x.GetById(It.IsAny<int>()))
                .Throws<Exception>();

            var vehicleService = _app.Services.GetRequiredService<IVehicleService>();

            // Act & Assert
            Assert.Throws<Exception>(() => vehicleService.GetById(999));
            _vehicleServiceMock.Verify(x => x.GetById(999), Times.Once);
        }

        // Helper method to create a mock HttpContext
        private static DefaultHttpContext CreateMockHttpContext()
        {
            return new DefaultHttpContext();
        }
    }
}