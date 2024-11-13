using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VehicleRegistration.Domain.Validation;

public class PasswordValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string password) return false;

        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        return regex.IsMatch(password);
    }

    public override string FormatErrorMessage(string name)
    {
        return "Password must contain at least 8 characters, one uppercase letter, one lowercase letter, one number and one special character";
    }
}