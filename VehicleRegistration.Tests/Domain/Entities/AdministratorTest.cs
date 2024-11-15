using System.ComponentModel.DataAnnotations;
using VehicleRegistration.Domain.Entities;

namespace VehicleRegistration.Tests.Domain.Entities
{
    public class AdministratorTest
    {
        [Fact]
        public void Administrator_ValidProperties_PassesValidation()
        {
            // Arrange
            var admin = new Administrator
            {
                Email = "admin@example.com",
                Password = "SecurePass123",
                Profile = "Admin"
            };

            // Act
            var validationContext = new ValidationContext(admin);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(admin, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData("admin@example.com", "", "Admin", "The Password field is required")]
        [InlineData("admin@example.com", "Password123", "", "The Profile field is required")]
        public void Administrator_InvalidProperties_FailsValidation(
            string email,
            string password,
            string profile,
            string expectedError)
        {
            // Arrange
            var admin = new Administrator
            {
                Email = email,
                Password = password,
                Profile = profile
            };

            // Act
            var validationContext = new ValidationContext(admin);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(admin, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage.Contains(expectedError));
        }

        [Fact]
        public void Administrator_IdGeneration_ShouldBeAutoGenerated()
        {
            // Arrange & Act
            var admin = new Administrator
            {
                Email = "admin@example.com",
                Password = "SecurePass123",
                Profile = "Admin"
            };

            // Assert
            Assert.Equal(0, admin.Id); // Id should be 0 before saving to database
                                       // Note: Testing actual ID generation would require integration tests with a database
        }

        [Theory]
        [InlineData("admin@example.com")]
        [InlineData("user.name@company.com")]
        [InlineData("test.user@subdomain.domain.com")]
        public void Administrator_ValidEmails_ShouldPass(string email)
        {
            // Arrange
            var admin = new Administrator
            {
                Email = email,
                Password = "SecurePass123",
                Profile = "Admin"
            };

            // Act
            var validationContext = new ValidationContext(admin);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(admin, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Administrator_PasswordLength_ShouldNotExceedMaxLength()
        {
            // Arrange
            var admin = new Administrator
            {
                Email = "admin@example.com",
                Password = new string('A', 51), // 51 characters, exceeding the 50 char limit
                Profile = "Admin"
            };

            // Act
            var validationContext = new ValidationContext(admin);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(admin, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage.Contains("maximum length"));
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Manager")]
        public void Administrator_ValidProfiles_ShouldPass(string profile)
        {
            // Arrange
            var admin = new Administrator
            {
                Email = "admin@example.com",
                Password = "SecurePass123",
                Profile = profile
            };

            // Act
            var validationContext = new ValidationContext(admin);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(admin, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }
    }
}