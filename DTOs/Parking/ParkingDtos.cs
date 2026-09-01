using System.ComponentModel.DataAnnotations;
namespace EventParkingReservationSystem.API.DTOs.Parking;
public class CreateParkingLayoutDto
{
    [Range(1, 1000)] public int SlotCount { get; set; }
    [Required, MaxLength(10)] public string Prefix { get; set; } = "P";
    [MaxLength(50)] public string? Zone { get; set; }
    [Range(0, double.MaxValue)] public decimal Fee { get; set; }
}
public class UpdateParkingSlotDto
{
    [Required, MaxLength(20)] public string SlotNumber { get; set; } = string.Empty;
    [MaxLength(50)] public string? Zone { get; set; }
    [Range(0, double.MaxValue)] public decimal? EventParkingFee { get; set; }
}

public record ParkingSlotResponseDto(
    int ParkingSlotId,
    string SlotNumber,
    string? Zone, 
    string Status,
    decimal Fee);

public class AddParkingDto
{
    [Range(1, int.MaxValue)]
    public int ParkingSlotId { get; set; }
}
