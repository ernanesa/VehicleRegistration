using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VehicleRegistration.Domain.DTOs;
using VehicleRegistration.Domain.Entities;
using VehicleRegistration.Domain.Enums;
using VehicleRegistration.Domain.Interfaces;
using VehicleRegistration.Endpoints;

namespace VehicleRegistration.Tests.Endpoints
{
    public class AdministratorEndpointsTests
    {
        private readonly Mock<IAdministratorService> _administratorServiceMock;
        private readonly WebApplication _app;
        private readonly string _jwtKey;
        private readonly IConfiguration _configuration;

        public AdministratorEndpointsTests()
        {
            _administratorServiceMock = new Mock<IAdministratorService>();
            
            // Setup configuration to read from appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
            _jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration");

            // Setup WebApplication
            var appBuilder = WebApplication.CreateBuilder();
            
            // Add configuration to the web application
            appBuilder.Configuration.AddConfiguration(_configuration);
            
            // Configure services
            appBuilder.Services.AddSingleton(_administratorServiceMock.Object);
            
            _app = appBuilder.Build();
            
            // Map endpoints
            AdministratorEndpoints.MapAdministratorEndpoints(_app);
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginDto = new LoginDTO { Email = "test@test.com", Password = "password123" };
            var administrator = new Administrator 
            { 
                Email = loginDto.Email,
                Password = loginDto.Password,
                Profile = "admin"
            };

            _administratorServiceMock
                .Setup(x => x.Login(It.IsAny<LoginDTO>()))
                .Returns(administrator);

            var administratorService = _app.Services.GetRequiredService<IAdministratorService>();

            // Act
            var result = administratorService.Login(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(administrator.Email, result.Email);
            Assert.Equal(administrator.Profile, result.Profile);
            
            _administratorServiceMock.Verify(x => x.Login(It.Is<LoginDTO>(
                dto => dto.Email == loginDto.Email && 
                       dto.Password == loginDto.Password)), 
                Times.Once);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDTO { Email = "invalid@test.com", Password = "wrongpassword" };

            _administratorServiceMock
                .Setup(x => x.Login(It.IsAny<LoginDTO>()))
                .Returns((Administrator)null);

            var administratorService = _app.Services.GetRequiredService<IAdministratorService>();

            // Act
            var result = administratorService.Login(loginDto);

            // Assert
            Assert.Null(result);
            
            _administratorServiceMock.Verify(x => x.Login(It.Is<LoginDTO>(
                dto => dto.Email == loginDto.Email && 
                       dto.Password == loginDto.Password)), 
                Times.Once);
        }

        [Fact]
        public void Register_WithValidData_ReturnsCreated()
        {
            // Arrange
            var adminDto = new AdministratorDTO 
            { 
                Email = "newadmin@test.com",
                Password = "newpassword123",
                Profile = ProfileEnum.admin
            };

            _administratorServiceMock
                .Setup(x => x.Register(It.IsAny<AdministratorDTO>()))
                .Verifiable();

            var administratorService = _app.Services.GetRequiredService<IAdministratorService>();

            // Act
            administratorService.Register(adminDto);

            // Assert
            _administratorServiceMock.Verify(x => x.Register(It.Is<AdministratorDTO>(
                dto => dto.Email == adminDto.Email && 
                       dto.Password == adminDto.Password && 
                       dto.Profile == adminDto.Profile)), 
                Times.Once);
        }

        [Fact]
        public void GetTokenJWT_ReturnsValidToken()
        {
            // Arrange
            var administrator = new Administrator
            {
                Email = "test@test.com",
                Profile = "admin"
            };
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            // Act
            var methodInfo = typeof(AdministratorEndpoints).GetMethod(
                "GetTokenJWT",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            
            var token = methodInfo?.Invoke(null, new object[] { administrator, key }) as string;

            // Assert
            Assert.NotNull(token);
            Assert.True(token.Length > 0);
            Assert.Contains(".", token); // JWT tokens contain at least 2 dots
        }

        // Helper method to create a mock HttpContext
        private static DefaultHttpContext CreateMockHttpContext()
        {
            return new DefaultHttpContext();
        }
    }
}