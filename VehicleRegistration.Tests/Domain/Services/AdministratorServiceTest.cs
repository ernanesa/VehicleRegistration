using Moq;
using VehicleRegistration.Domain.DTOs;
using VehicleRegistration.Domain.Entities;
using VehicleRegistration.Domain.Services;
using VehicleRegistration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using VehicleRegistration.Domain.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace VehicleRegistration.Tests.Domain.Services;

public class AdministratorServiceTests
{
    private readonly Mock<DbSet<Administrator>> _mockSet;
    private readonly VehicleRegistrationContext _context;
    private readonly AdministratorService _service;
    private readonly List<Administrator> _administrators;

    public AdministratorServiceTests()
    {
        _administrators = new List<Administrator>
        {
            new Administrator
            {
                Id = 1,
                Email = "admin@test.com",
                Password = "password123",
                Profile = "admin"
            }
        };

        _mockSet = CreateMockDbSet(_administrators);

        var options = new DbContextOptionsBuilder<VehicleRegistrationContext>()
            .UseInMemoryDatabase(databaseName: "VehicleRegistrationTest")
            .Options;

        _context = new VehicleRegistrationContext(options);

        var property = _context.GetType().GetProperty("Administrators");
        property?.SetValue(_context, _mockSet.Object);

        _service = new AdministratorService(_context);
    }

    private static Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> elements) where T : class
    {
        var elementsAsQueryable = elements.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => elementsAsQueryable.GetEnumerator());

        return mockSet;
    }

    [Fact]
    public void Login_WithValidCredentials_ReturnsAdministrator()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            Email = "admin@test.com",
            Password = "password123"
        };

        // Act
        var result = _service.Login(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(loginDto.Email, result.Email);
        Assert.Equal(loginDto.Password, result.Password);
    }

    [Fact]
    public void Login_WithInvalidCredentials_ReturnsNull()
    {
        // Arrange
        var loginDto = new LoginDTO
        {
            Email = "wrong@test.com",
            Password = "wrongpassword"
        };

        // Act
        var result = _service.Login(loginDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Register_ValidAdministrator_SavesToDatabase()
    {
        // Arrange
        var adminDto = new AdministratorDTO
        {
            Email = "newadmin@test.com",
            Password = "newpassword123",
            Profile = ProfileEnum.admin
        };

        Administrator savedAdmin = null;
        _mockSet.Setup(x => x.Add(It.IsAny<Administrator>()))
            .Callback<Administrator>(admin => savedAdmin = admin)
            .Returns((EntityEntry<Administrator>)null);

        // Act
        _service.Register(adminDto);

        // Assert
        _mockSet.Verify(x => x.Add(It.IsAny<Administrator>()), Times.Once);

        Assert.NotNull(savedAdmin);
        Assert.Equal(adminDto.Email, savedAdmin.Email);
        Assert.Equal(adminDto.Password, savedAdmin.Password);
        Assert.Equal(adminDto.Profile.ToString(), savedAdmin.Profile);
    }
}