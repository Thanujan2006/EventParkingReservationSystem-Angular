using System.ComponentModel.DataAnnotations;
using EventParkingReservationSystem.API.Enums;

namespace EventParkingReservationSystem.API.Models;

public class Booking
{
    public int BookingId { get; set; }

    [Required, MaxLength(30)]
    public string BookingNumber { get; set; } = string.Empty;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime? HoldExpiresAtUtc { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? ConfirmedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }

    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    public ICollection<ParkingReservation> ParkingReservations { get; set; } = new List<ParkingReservation>();
    public Payment? Payment { get; set; }
}
