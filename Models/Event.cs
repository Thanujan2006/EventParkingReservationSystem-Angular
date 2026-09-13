using System.ComponentModel.DataAnnotations;

namespace EventParkingReservationSystem.API.Models;

public class Event
{
    public int EventId { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public int VenueId { get; set; }
    public Venue Venue { get; set; } = null!;

    public int EventCategoryId { get; set; }
    public EventCategory EventCategory { get; set; } = null!;

    public DateOnly EventDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TicketPrice { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ParkingFee { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();


}
