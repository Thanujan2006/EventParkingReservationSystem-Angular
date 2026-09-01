using EventParkingReservationSystem.API.Enums;

namespace EventParkingReservationSystem.API.Models;


public class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
    public DateTime PaidAtUtc { get; set; } = DateTime.UtcNow;
}
