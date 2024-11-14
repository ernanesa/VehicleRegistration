using System.ComponentModel.DataAnnotations;
using VehicleRegistration.Domain.Entities;

namespace VehicleRegistration.Tests.Domain.Entities
{
    public class VehicleTests
    {
        [Fact]
        public void Vehicle_ValidProperties_ShouldPassValidation()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Name = "Civic",
                Brand = "Honda",
                Year = 2024
            };

            // Act
            var validationContext = new ValidationContext(vehicle);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(vehicle, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void Vehicle_EmptyName_ShouldFailValidation(string invalidName)
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Name = invalidName,
                Brand = "Honda",
                Year = 2024
            };

            // Act
            var validationContext = new ValidationContext(vehicle);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(vehicle, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Vehicle_NameExceedsMaxLength_ShouldFailValidation()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Name = new string('A', 151), // 151 characters, exceeding the 150 limit
                Brand = "Honda",
                Year = 2024
            };

            // Act
            var validationContext = new ValidationContext(vehicle);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(vehicle, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void Vehicle_EmptyBrand_ShouldFailValidation(string invalidBrand)
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Name = "Civic",
                Brand = invalidBrand,
                Year = 2024
            };

            // Act
            var validationContext = new ValidationContext(vehicle);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(vehicle, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Brand"));
        }

        [Fact]
        public void Vehicle_BrandExceedsMaxLength_ShouldFailValidation()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Name = "Civic",
                Brand = new string('A', 101), // 101 characters, exceeding the 100 limit
                Year = 2024
            };

            // Act
            var validationContext = new ValidationContext(vehicle);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(vehicle, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Brand"));
        }

        [Fact]
        public void Vehicle_DefaultId_ShouldBeZero()
        {
            // Arrange & Act
            var vehicle = new Vehicle();

            // Assert
            Assert.Equal(0, vehicle.Id);
        }
    }
}