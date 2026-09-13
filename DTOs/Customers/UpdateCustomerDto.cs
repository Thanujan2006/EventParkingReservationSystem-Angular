using System.ComponentModel.DataAnnotations;
namespace EventParkingReservationSystem.API.DTOs.Customers;
public class UpdateCustomerDto
{
    [Required, MinLength(2), MaxLength(150)] public string Name { get; set; } = string.Empty;
    [Required, RegularExpression(@"^\d{10}$")] public string Phone { get; set; } = string.Empty;
}
