using EventParkingReservationSystem.API.Enums;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Enums;

namespace EventParkingReservationSystem.API.Models;

public class Seat
{
    public int SeatId { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    [Required, MaxLength(20)]
    public string SeatNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? SeatType { get; set; }

    public decimal? Price { get; set; }

    public SeatStatus Status { get; set; } = SeatStatus.Available;

    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
}
