using System.ComponentModel.DataAnnotations;
namespace EventParkingReservationSystem.API.DTOs.Customers;
public class RegisterCustomerDto
{
    [Required, MinLength(2), MaxLength(150)] public string Name { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(200)] public string Email { get; set; } = string.Empty;
    [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must contain exactly 10 digits.")] public string Phone { get; set; } = string.Empty;
    [Required, MinLength(8), RegularExpression(@"^(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$", ErrorMessage = "Password must contain at least one number and one special character.")] public string Password { get; set; } = string.Empty;
    [Required, Compare(nameof(Password))] public string ConfirmPassword { get; set; } = string.Empty;
}


