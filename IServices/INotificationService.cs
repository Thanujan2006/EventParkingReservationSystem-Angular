using EventParkingReservationSystem.API.DTOs.Notifications;
using EventParkingReservationSystem.API.Enums;

namespace EventParkingReservationSystem.API.Services;

public interface INotificationService
{
    Task CreateAsync(int customerId, NotificationType type, string message);
    Task<IReadOnlyList<NotificationResponseDto>> GetForCustomerAsync(int customerId);
    Task MarkReadAsync(int customerId, int notificationId);
}
