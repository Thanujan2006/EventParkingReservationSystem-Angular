using System.ComponentModel.DataAnnotations;
using EventParkingReservationSystem.API.Enums;

namespace EventParkingReservationSystem.API.Models;

public class Notification
{
    public int NotificationId { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public NotificationType Type { get; set; }

    [Required, MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
