using EventParkingReservationSystem.API.Enums;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Enums;

namespace EventParkingReservationSystem.API.Models;

public class ParkingSlot
{
    public int ParkingSlotId { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    [Required, MaxLength(20)]
    public string SlotNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Zone { get; set; }

    public ParkingSlotStatus Status { get; set; } = ParkingSlotStatus.Available;

    public ICollection<ParkingReservation> ParkingReservations { get; set; } = new List<ParkingReservation>();
}
