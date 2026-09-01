namespace EventParkingReservationSystem.API.Models;

public class ParkingReservation
{
    public int ParkingReservationId { get; set; }

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public int ParkingSlotId { get; set; }
    public ParkingSlot ParkingSlot { get; set; } = null!;

    public decimal FeeAtReservation { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime ReservedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ReleasedAtUtc { get; set; }
}
