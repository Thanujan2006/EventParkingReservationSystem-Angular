namespace EventParkingReservationSystem.API.DTOs.Notifications;
public record NotificationResponseDto(int NotificationId, string Type, string Message, bool IsRead, DateTime CreatedAtUtc);
