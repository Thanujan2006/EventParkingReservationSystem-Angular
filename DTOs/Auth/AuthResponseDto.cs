namespace EventParkingReservationSystem.API.DTOs.Auth;
public record AuthResponseDto(int UserId, string Name, string Email, string Role, string Token);
