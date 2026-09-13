using System.ComponentModel.DataAnnotations;
namespace EventParkingReservationSystem.API.DTOs.Auth;
public class ForgotPasswordDto
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
}

